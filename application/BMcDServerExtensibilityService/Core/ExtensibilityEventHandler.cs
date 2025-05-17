using Amqp;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BMcDExtensibilityService.Core
{
    /// <summary>
    /// Responsible for filtering the subject and calling the appropriate class 
    /// and method with the event data
    /// </summary>
    public class ExtensibilityEventHandler
    {        

        private List<string> crudEventTypes = new List<string>() { "OnCreated", "OnDeleted", "OnTerminated", "OnCopied", "OnUpdated" };
        private List<string> workflowEventTypes = new List<string>() { "Execute" };
        private List<string> relEventTypes = new List<string>() { "OnRelCreated", "OnRelDeleted", "OnRelTerminated", "OnRelCopied", "OnRelUpdated", "OnRelCopied" };
               

        public void ProcessEventMessage(ExtensibilityODataClient extensibilityODataClient, string subject, object body)
        {
            try
            {
                // get components of subject annotation which will help up work out if the message
                // is relevant without the overhead of deserializing the payload
                // component 1 is always product e.g. "sdx"
                // component 2 is always be the server name e.g. "spfserver"
                // component 3 is always the event name "OnCreated"
                // The remaining components are depending on the event type and they will be documented in fluid topics
                string[] subjectArray = subject.Split('/');
                string eventType = subjectArray[2];

                
                //Log.Information("Event received: " + eventType);

                List<string> applicableFiles = new List<string>();

                if (crudEventTypes.Contains(eventType) || relEventTypes.Contains(eventType))
                {
                    // Crud events
                    string classDefUID = subjectArray[4];
                    applicableFiles.Add(classDefUID);
                    string[] interfaceUIDs = subjectArray[5].Split(',');
                    applicableFiles.AddRange(interfaceUIDs);
                    CallCustomCode(extensibilityODataClient, eventType, applicableFiles, body);
                }
                else if (workflowEventTypes.Contains(eventType))
                {
                    // Custom process step events
                    string workflowStepClassName = subjectArray[4];
                    Log.Debug(eventType + " WofkFlow event" + workflowStepClassName);
                    applicableFiles.Add(workflowStepClassName);
                    CallCustomCode(extensibilityODataClient, eventType, applicableFiles, body);
                }
                else
                {
                    Log.Information(eventType + " Event type not handled");
                    Log.Debug(eventType + " Event type not handled");
                }

            }
            catch (Exception ex)
            {
                Log.Information(ex, "Error in handling event from server");
            }

        }

        private static void CallCustomCode(ExtensibilityODataClient extensibilityODataClient, string eventType, List<string> filesToMatch, object body)
        {
            // Reflect class loaded into project to obtain list of classes
            foreach (Assembly item in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in item.GetTypes().Where(x => filesToMatch.Contains(x.Name)))
                {
                    if (type != null)
                    {
                        // If class and method found which matches event then create an instance and call it
                        object instance = Activator.CreateInstance(type);
                        MethodInfo lobjMethod = type.GetMethod(eventType);
                        if (lobjMethod != null)
                        {
                            lobjMethod.Invoke(instance, new object[] { extensibilityODataClient, body });
                        }
                    }
                }               
            }
        }
        
    }
}
