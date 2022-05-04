using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Wi.Dpi.Validations.Models
{
    /// <summary>
    /// Logic which identifies errors within the data or which require further investigation.
    /// </summary>
    public class ValidationRule
    {
        public string Id { get; set; }

        /// <summary>
        /// This is the unique Id for a validation rule.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RuleIdentifier { get; set; }

        /// <summary>
        /// The source or origin of the rule.
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RuleSource { get; set; }

        /// <summary>
        /// A link to more information about the rule and how to resolve it.
        /// </summary>
        [MaxLength(255)]
        public string HelpUrl { get; set; }

        /// <summary>
        /// This is non-structured ASCII text that will include the short details that were used in the evaluation of the validation rule.
        /// </summary>
        [MaxLength(1024)]
        public string ShortDescription { get; set; }

        /// <summary>
        ///  This is non-structured ASCII text that will include the details that were used in the evaluation of the validation rule.
        /// </summary>
        [Required]
        [MaxLength(4000)]
        public string Description { get; set; }

        /// <summary>
        /// The current status of if the rule. Examples are 'Active', 'Under Analysis', 'Inactive', 'Deprecated'.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string RuleStatusDescriptor { get; set; }

        /// <summary>
        ///  This is a category for the type of validation rule.  Examples might be 'Student Demographics', 'Special Education', or 'Attendance' 
        /// </summary>
        [MaxLength(1024)]
        public string Category { get; set; }

        /// <summary>
        ///  This specifies whether the validation rule is a 'Warning', 'Minor Validation Error', 'Major Validation Error' or other value standardized by the API
        /// </summary>
        [Required]
        [MaxLength(306)]
        public string SeverityDescriptor { get; set; }

        /// <summary>
        /// Refers back to a unique identifier for this rule in another system (such as a state-maintained repository of validation rules)
        /// </summary>
        [MaxLength(100)]
        public string ExternalRuleId { get; set; }

        /// <summary>
        /// Specifies the language that the validation logic is represented in, ie SQL or Pseudo-code
        /// </summary>
        [MaxLength(306)]
        public string ValidationLogicTypeDescriptor { get; set; }

        /// <summary>
        /// Has the actual code or pseudo-code that is used to find validation errors.
        /// </summary>
        [MaxLength(4000)]
        public string ValidationLogic { get; set; }

       

    }

}
