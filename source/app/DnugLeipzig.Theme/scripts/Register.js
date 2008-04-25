var Register = new Object();

Register.submitMessage = function(url)
{
    GraffitiHelpers.statusMessage('register_status', 'sending', true);
 
    new Ajax.Request(url + '?command=register',
    {
        method: 'post',
        parameters: Form.serialize('registationForm'),
        onSuccess: function(transport)
                    {
                         var response = transport.responseText || "no response text";
                         GraffitiHelpers.statusMessage('registration_status', response, true);
                         $('message').value = '';
                     },
        onFailure: function()
        {
            GraffitiHelpers.statusMessage('contact_status', 'Something went wrong. The registration request was likely not sent.', true);
        }
  });
}