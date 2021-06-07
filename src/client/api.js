function handleErrors(response) {
	if (!response.ok) {
		throw Error(response.statusText);
	}
	return response;
}

function getValues() {
	return fetch('/api/apex')
		.then(handleErrors)
		.then(response => response.json())
		.catch(err => alert(err));
}

function getFiles() {
	return fetch('/api/apex')
		.then(handleErrors)
		.then(response => response.json())
		.catch(err => alert(err));
}

export default {
	getValues
};
