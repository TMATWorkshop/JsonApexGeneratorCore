function handleErrors(response) {
	if (!response.ok) {
		throw Error(response.statusText);
	}
	return response;
}

function generateFiles(className, namedCredential, calloutMethod, requestJSON, responseJSON) {
	const requestOptions = {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ className: className,
								namedCredential: namedCredential,
								calloutMethod: calloutMethod,
								requestJSON: requestJSON,
								responseJSON: responseJSON })
    };
	fetch('/api/apex/', requestOptions)
		.then(handleErrors)
		.then(r => r.blob())
		.then(response => {
			const element = document.createElement("a");
			const file = new Blob([response], {type: 'application/zip'});
			element.href = URL.createObjectURL(file);
			element.download = className + '.zip';
			document.body.appendChild(element); // Required for this to work in FireFox
			element.click();
		})
		.catch(err => alert(err));
	/*
	fetch('/api/apex/wrapper', requestOptions)
		.then(handleErrors)
		.then(r => r.blob())
		.then(response => {
			const element = document.createElement("a");
			const file = new Blob([response], {type: 'text/plain'});
			element.href = URL.createObjectURL(file);
			element.download = className + 'Wrapper.apxc';
			document.body.appendChild(element); // Required for this to work in FireFox
			element.click();
		})
		.catch(err => alert(err));
		*/
}

export default {
	generateFiles
};
