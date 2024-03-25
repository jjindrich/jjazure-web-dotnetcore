using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Plugins.Core;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using System.Threading.Tasks;
using System.ComponentModel;

namespace jjwebcore.KernelFunctions
{
    public class AgePlugin
    {
        [KernelFunction]
        [Description("Returns the current age of person.")]
        [return: Description("Age of person")]
        public async Task<string> GetCurrentAgeAsync(
                   Kernel kernel,
                   [Description("Person name")] string personName
               )
        {
            // Add logic to get age
            if (personName.Contains("Pavel"))
            {
                return "The current age is 25.";
            }
            else
            {
                return "The current age is 30.";
            }            
        }
    }
}
