const url = "https://localhost:5001/api/beanvariety/";

const body = document.querySelector('.main')
const runButton = document.querySelector("#run-button");
const beanContainer = document.querySelector(".beanContainer")
const addBeanButton = document.querySelector('#addNewBean-button')
const newBeanFormContainer = document.querySelector('.newBeanFormContainer')
const submitNewBeanBtn = document.querySelector('submitNewBean-button')


addBeanButton.addEventListener('click', () => {
    newBeanForm()
    newBeanFormContainer.style.display = "block"
})

const newBeanForm = () => newBeanFormContainer.innerHTML = `
<form class="newBeanForm">
<fieldset class="newBeanForm__fieldset">
<label for="beanName">Bean Name</label>
<input class="beanName" type"text" name="beanName">
<label for="beanRegion">Region</label>
<input class="beanRegion" type="text" name="beanRegion">
<label for="beanNotes">Notes</label>
<textarea class="beanNotes" name="notes" cols="10" rows="5"></textarea>
<button class="submitNewBean-button">Submit New Bean</button>
</fieldset>
</form>
`

const beanHtmlGenerator = (beanVarieties) => {

    const beanHtml = beanVarieties.map(v => `
            <fieldset style="margin: 10px;">
            <h3>${v.name}</h3>
            <h4>Region: ${v.region}</h4>
            <p>Notes: ${v.notes}</p>
            <button value=${v.id} class="deleteBeanVariety-button">Delete</button>
            </fieldset>
        `

    ).join('')

    beanContainer.innerHTML += beanHtml

}

body.addEventListener('click', e => {
    if (e.target.classList.contains('deleteBeanVariety-button')) {
        const id = e.target.value
        deleteBeanVariety(id)
    }
})



runButton.addEventListener("click", () => {
    getAllBeanVarieties()
        .then(beanVarieties => {
            beanHtmlGenerator(beanVarieties);
        })
});

function getAllBeanVarieties() {
    return fetch(url).then(resp => resp.json());
}

const addNewBeanVariety = (newBeanVariety) => fetch(url, {
    method: "POST",
    headers: {
        "Content-Type": "application/json"
    },
    body: JSON.stringify(newBeanVariety)
}).then(getAllBeanVarieties)

const deleteBeanVariety = (id) => fetch(url + id, {
    method: 'DELETE'
})


body.addEventListener('click', (e) => {
    if (e.target.classList.contains('submitNewBean-button')) {

        // e.target.preventDefault()

        const newBeanVarietyObj = {
            name: document.querySelector('.beanName').value,
            region: document.querySelector('.beanRegion').value,
            notes: document.querySelector('.beanNotes').value
        }
        addNewBeanVariety(newBeanVarietyObj)

        newBeanFormContainer.style.display = 'none'
    }

})
