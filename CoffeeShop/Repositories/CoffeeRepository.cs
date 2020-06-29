using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using CoffeeShop.Models;

namespace CoffeeShop.Repositories
{
    public class CoffeeRepository
    {
        private readonly string _connectionString;
        public CoffeeRepository(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public SqlConnection Connection
        {
            get { return new SqlConnection(_connectionString); }
        }

        public List<Coffee> GetAll()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id as CoffeeId, c.Title as CoffeeTitle, c.BeanVarietyId, 
                                        b.Id as VarietyId, b.Name as BeanVarietyName, b.Region as BeanVarietyRegion, 
                                        b.Notes as BeanVarietyNotes FROM Coffee c
                                        JOIN BeanVariety b on c.BeanVarietyId = b.Id;";
                    var reader = cmd.ExecuteReader();
                    var coffeeList = new List<Coffee>();
                    BeanVariety variety = null;
                    while (reader.Read())
                    {
                        variety = new BeanVariety()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("VarietyId")),
                            Name = reader.GetString(reader.GetOrdinal("BeanVarietyName")),
                            Region = reader.GetString(reader.GetOrdinal("BeanVarietyRegion")),
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("BeanVarietyNotes")))
                        {
                            variety.Notes = reader.GetString(reader.GetOrdinal("BeanVarietyNotes"));
                        }
                        var coffee = new Coffee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CoffeeId")),
                            Title = reader.GetString(reader.GetOrdinal("CoffeeTitle")),
                            BeanVarietyId = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),
                            BeanVariety = variety
                        };
                        
                        coffeeList.Add(coffee);
                    }

                    reader.Close();

                    return coffeeList;
                }
            }
        }

        public Coffee Get(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT c.Id as CoffeeId, c.Title as CoffeeTitle, c.BeanVarietyId, 
                                        b.Id as BeanVarietyId, b.Name as BeanVarietyName, b.Region as BeanVarietyRegion, 
                                        b.Notes as BeanVarietyNotes FROM Coffee c
                                        JOIN BeanVariety b on c.BeanVarietyId = b.Id
                                        WHERE c.Id = @id;";
                    cmd.Parameters.AddWithValue("@id", id);

                    var reader = cmd.ExecuteReader();
                    Coffee coffee = null;
                    BeanVariety variety = null;
                    if (reader.Read())
                    {
                        variety = new BeanVariety()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),
                            Name = reader.GetString(reader.GetOrdinal("BeanVarietyName")),
                            Region = reader.GetString(reader.GetOrdinal("BeanVarietyRegion")),
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("BeanVarietyNotes")))
                        {
                            variety.Notes = reader.GetString(reader.GetOrdinal("BeanVarietyNotes"));
                        }
                        coffee = new Coffee()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CoffeeId")),
                            Title = reader.GetString(reader.GetOrdinal("CoffeeTitle")),
                            BeanVarietyId = reader.GetInt32(reader.GetOrdinal("BeanVarietyId")),
                            BeanVariety = variety
                        };
                    }

                    reader.Close();

                    return coffee;
                }
            }
        }

        public void Add(Coffee coffee)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Coffee ([Title], BeanVarietyId)
                        OUTPUT INSERTED.ID
                        VALUES (@title, @beanVarietyId)";
                    cmd.Parameters.AddWithValue("@title", coffee.Title);
                    cmd.Parameters.AddWithValue("@beanVarietyId", coffee.BeanVarietyId);

                    coffee.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Update(Coffee coffee)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE Coffee 
                           SET Title = @title, 
                               BeanVarietyId = @beanVarietyId 
                         WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", coffee.Id);
                    cmd.Parameters.AddWithValue("@title", coffee.Title);
                    cmd.Parameters.AddWithValue("@beanVarietyId", coffee.BeanVarietyId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Coffee WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}