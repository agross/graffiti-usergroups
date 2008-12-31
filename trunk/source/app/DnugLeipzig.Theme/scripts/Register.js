var Register = new Object();

Register.submitMessage = function(url)
{
	// Track this click in the registration funnel of Google Analytics.
	if (typeof (pageTracker) !== "undefined")
	{
		pageTracker._trackPageview("/funnel-registration/register-clicked.html")
	}

	Form.Element.disable('register');
	$('success').hide();
	$('error').hide();
	$$('span.status').each(function(e) { e.hide(); });
	$('working').show();
	GraffitiHelpers.statusMessage('registration-status', 'Sending request, please wait.', true);

	new Ajax.Request(url + '?command=register',
	{
		method: 'post',
		parameters: Form.serialize('registration'),
		onSuccess: function(transport)
		{
			var response = transport.responseText.evalJSON();

			if (!response.ValidationErrors)
			{
				response.each(function(event)
				{
					if (event.ErrorOccurred)
					{
						$('event-' + event.EventId + '-error').show();
					}
					else
					{
						if (event.AlreadyRegistered)
						{
							$('event-' + event.EventId + '-already-registered').show();
						}
						else
						{
							$('event-' + event.EventId + '-success').show();
						}
					}

					if (event.OnWaitingList)
					{
						$('event-' + event.EventId + '-waitinglist').show();
					}
				});

				GraffitiHelpers.statusMessage('registration-status', "Thank you for your registration.", false);

				if (response.any(function(event) { return event.OnWaitingList; }))
				{
					GraffitiHelpers.statusMessage('registration-status', $('registration-status').innerHTML + " You are on the waiting list for some events.", false);
				}

				if (response.any(function(event) { return event.ErrorOccurred; }))
				{
					GraffitiHelpers.statusMessage('registration-status', $('registration-status').innerHTML + " Some registration requests caused an error.", false);
				}
			}
			else
			{
				$('error').show();

				var message = 'An error has occured. Please check the following form fields:<ul>';
				response.ValidationErrors.each(function(item) { message += '<li>' + item + '</li>'; });
				message += '</ul>';

				GraffitiHelpers.statusMessage('registration-status', message, false);
			}
		},
		onFailure: function()
		{
			$('error').show();
			GraffitiHelpers.statusMessage('registration-status', 'An error occured while processing your request. It is likely that the request has not been sent.', true);
		},
		onComplete: function()
		{
			$('working').hide();
			Form.Element.enable('register');
		}
	});
}

var validation = new Validation('registration', { immediate: true });

Validation.add('select-event', null, function (v, elm)
	{
		var p = $('event-list');
		var options = p.getElementsByTagName('INPUT');
		return $A(options).any(function(elm)
		{
			return $F(elm);
		});
	});

$('register').observe('click', function(event)
	{
		if(validation.validate())
		{
			Register.submitMessage($('registration').action);
		}
		
		Event.stop(event);
	}
	, false);