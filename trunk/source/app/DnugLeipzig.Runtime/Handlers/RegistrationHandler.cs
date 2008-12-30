using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Castle.Core.Logging;

using DnugLeipzig.Definitions;
using DnugLeipzig.Definitions.Commands;
using DnugLeipzig.Definitions.Commands.Results;
using DnugLeipzig.Definitions.Extensions;

namespace DnugLeipzig.Runtime.Handlers
{
	public class RegistrationHandler : IHttpHandler
	{
		readonly ICommandFactory _commandFactory;
		ILogger _logger;

		public RegistrationHandler() : this(IoC.Resolve<ICommandFactory>(), IoC.Resolve<ILogger>())
		{
		}

		public RegistrationHandler(ICommandFactory commandFactory, ILogger logger)
		{
			_commandFactory = commandFactory;
			Logger = logger;
		}

		public ILogger Logger
		{
			get
			{
				if (_logger == null)
				{
					return NullLogger.Instance;
				}
				return _logger;
			}
			set { _logger = value; }
		}

		#region IHttpHandler Members
		public void ProcessRequest(HttpContext context)
		{
			if (context.Request.RequestType != "POST")
			{
				new ForbiddenResult().Render(context.Response);
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

						command = _commandFactory.EventRegistration(eventsToSubscribe,
						                                                    context.Request.Form["formOfAddress"],
						                                                    context.Request.Form["name"],
						                                                    context.Request.Form["occupation"],
						                                                    context.Request.Form["attendeeEMail"],
						                                                    context.Request.Form["ccToAttendee"].IsChecked());
						break;

					default:
						throw new InvalidOperationException(String.Format("Unknown command '{0}'", context.Request.QueryString["command"]));
				}

				IHttpResponse result = command.Execute();
				result.Render(context.Response);
			}
			catch (Exception ex)
			{
				Logger.Error(Create.New.Message().WithTitle("Could not process registration request"), ex);
				new ErrorResult().Render(context.Response);
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}
		#endregion
	}
}