using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.Definitions.Extensions;

using Graffiti.Core;

namespace DnugLeipzig.Runtime.Handlers
{
	public class RegistrationHandler : IHttpHandler
	{
		readonly ICommandFactory _commandFactory;

		public RegistrationHandler() : this(IoC.Resolve<ICommandFactory>())
		{
		}

		public RegistrationHandler(ICommandFactory commandFactory)
		{
			_commandFactory = commandFactory;
		}

		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
		{
			if (context.Request.RequestType != "POST")
			{
				new ForbiddenResult().Render(HttpContext.Current.Response);
				return;
			}

			try
			{
				ICommand command;
				switch (context.Request.QueryString["command"])
				{
					case "register":
						IEnumerable<int> eventsToSubscribe = from key in context.Request.Form.AllKeys
						                                     where key != null && key.StartsWith("event-")
						                                     select Convert.ToInt32(key.Replace("event-", String.Empty));

						command = _commandFactory.MultipleEventRegistration(eventsToSubscribe,
						                                                    context.Request.Form["formOfAddress"],
						                                                    context.Request.Form["name"],
						                                                    context.Request.Form["occupation"],
						                                                    context.Request.Form["attendeeEMail"],
						                                                    context.Request.Form["ccToAttendee"].IsChecked());
						break;

					default:
						throw new InvalidOperationException(String.Format("Unknown command '{0}'", context.Request.QueryString["command"]));
				}

				ICommandResult result = command.Execute();
				result.Render(context.Response);
			}
			catch (Exception ex)
			{
				// TODO: Inject
//				Log.Error(String.Format("{0}: Could not process registration request", GetType().Name), ex.ToString());
				new ErrorResult().Render(HttpContext.Current.Response);
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}
		#endregion
	}
}