using EdFi.Ods.Api.Models;
using Newtonsoft.Json;

namespace Wi.Dpi.Validations.Models
{
    public class ValidationRuleRunReference
    {
        [JsonIgnore]
        public string Id { get; set; }
        /// <summary>
        /// This is a unique Id for each run
        /// </summary>
        public string RunIdentifier { get; set; }
        public Link Link
        {
            get
            {
                return new Link
                {
                    Rel = "ValidationRuleRun",
                    Href = $"/ed-fi-xvalidations/validationRuleRuns/{Id}"
                };
            }
        }

    }

}
