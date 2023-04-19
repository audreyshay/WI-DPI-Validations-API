using EdFi.Ods.Api.Models;
using Newtonsoft.Json;

namespace Wi.Dpi.Validations.Models
{
    public class ValidationRuleReference
    {
        [JsonIgnore]
        public string Id { get; set; }
        /// <summary>
        /// The unique Id for a validation rule.
        /// </summary>
        public string RuleIdentifier { get; set; }
        /// <summary>
        /// The source or origin of the rule.
        /// </summary>
        public string RuleSource { get; set; }
        public Link Link
        {
            get
            {
                return new Link
                {
                    Rel = "ValidationRule",
                    Href = $"/ed-fi-xvalidations/validationRules/{Id}"
                };
            }
        }

    }

}
