var Register = new Object();

Register.submitMessage = function(url)
{
	GraffitiHelpers.statusMessage('registration-status', 'Sende Anfrage... Bitte warten.', true);
	
	new Ajax.Request(url + '?command=register',
	{
		method: 'post',
		parameters: Form.serialize('registationForm'),
		onSuccess: function(transport)
		{
			var response = transport.responseText || "Keine Antwort";
			GraffitiHelpers.statusMessage('registration-status', response, true);
		},
		onFailure: function()
		{
			GraffitiHelpers.statusMessage('registration-status', 'Beim Verarbeiten der Anforderung ist ein Fehler aufgetreten. Die Anmeldung wurde wahrscheinlich nicht gesendet.', true);
		}
	});
}