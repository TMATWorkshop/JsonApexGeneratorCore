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

function generateFiles(className, namedCredential, requestJSON, responseJSON) {
	const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ className: className,
								namedCredential: namedCredential,
								requestJSON: requestJSON,
								responseJSON: responseJSON })
    };
	return fetch('/api/apex', requestOptions)
		.then(handleErrors)
		.then(response => {
			const element = document.createElement("a");
			const file = new Blob([response], {type: 'text/plain'});
			element.href = URL.createObjectURL(file);
			element.download = className + '.apxc';
			document.body.appendChild(element); // Required for this to work in FireFox
			element.click();
		})
		.catch(err => alert(err));
}

export default {
	getValues,generateFiles
};
