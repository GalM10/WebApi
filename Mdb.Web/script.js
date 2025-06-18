const userList = document.getElementById('userList');
const age = document.getElementById('age');
const firstName = document.getElementById('firstName');
const lastName = document.getElementById('lastName');
const submitButton = document.getElementById('submitButton');
const searchField = document.getElementById('searchField');
const searchButton = document.getElementById('searchButton');
const clearSearchButton = document.getElementById('clearSearchButton');
const userSearchList = document.getElementById('userSearchList');

const URL = 'http://localhost:5101/api/users';

window.addEventListener("load", async () => {
    const users = await loadUsers();
    displayUsers(users);
});

async function createUser(userData)
{
    await fetch(URL,
    {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(userData)
    });
}

async function loadUsers()
{
    const response = await fetch(URL,{
        method: 'GET'
    });
    return await response.json();
}

async function deleteUser(userId)
{
    const response = await fetch(`${URL}/${userId}`,{
        method: 'DELETE'
    });
    return response.ok;
}


async function searchUsers(search)
{
    const response = await fetch(`${URL}/search/${encodeURIComponent(search)}`,{
        method: 'GET'
    });

    return await response.json();
}

function displaySearchResults(users)
{
    userSearchList.innerHTML = users.map(u =>
        `<li>
            <span>${u.firstName} ${u.lastName} ${u.age}</span>
        </li>`
    ).join('');
}

function displayUsers(users) {
    userList.innerHTML = users.map(u =>
        `<li>
            <span>${u.firstName} ${u.lastName} ${u.age}</span>
            <button class="delete-btn" data-id="${u.id}">Удалить</button>
        </li>`
    ).join('');

    document.querySelectorAll('.delete-btn').forEach(delButton =>
    {
        delButton.addEventListener('click', async (event) =>
        {
            let userId = event.target.getAttribute('data-id');
            let isDeleted = await deleteUser(userId);
            if (isDeleted) {
                let users = await loadUsers();
                displayUsers(users);
            }
        })
    });
}

searchButton.addEventListener('click', async (event) =>
{
    event.preventDefault();

    const searchTerm = searchField.value;
    if (!searchTerm) {
        return;
    }

    const users = await searchUsers(searchTerm);
    displaySearchResults(users);
});

submitButton.addEventListener('click', async (event) =>
{
    event.preventDefault();
    if (!firstName.value || !lastName.value || !age.value)
    {
        return;
    }

    const user =
    {
        firstName: firstName.value,
        lastName: lastName.value,
        age: age.value,
    };

    await createUser(user);
    firstName.value = '';
    lastName.value = '';
    age.value = '';
    const users = await loadUsers();
    displayUsers(users);
 });

 clearSearchButton.addEventListener('click', (event) => {
    event.preventDefault();
    clearSearchResults();
});

function clearSearchResults() {
    userSearchList.innerHTML = '';
    searchField.value = '';
}

