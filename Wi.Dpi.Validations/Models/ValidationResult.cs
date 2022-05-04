using EdFi.Ods.Api.Common.Models.Resources.EducationOrganization.EdFi;
using EdFi.Ods.Api.Common.Models.Resources.Staff.EdFi;
using EdFi.Ods.Api.Common.Models.Resources.Student.EdFi;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Wi.Dpi.Validations.Models
{
    /// <summary>
    /// This is the actual results from the validation rule.
    /// </summary>
    public class ValidationResult
    {
        public string Id { get; set; }

        /// <summary>
        /// This is a unique id for the validation result.
        /// </summary>
        [Required]
        [MaxLength(100)]
       public string ResultIdentifier { get; set; }

        /// <summary>
        /// This refers (foreign key) back up to the validation rule run.
        /// </summary>
        [Required]
        public ValidationRuleReference ValidationRuleReference { get; set; }

        /// <summary>
        /// This is a unique id that points back to the validation rule that caused the result to be produced.
        /// </summary>
         public ValidationRuleRunReference ValidationRuleRunReference { get; set; }

        /// <summary>
        /// This is the unique identifier in the ODS that is used to reference a specific resource.  Examples include an Id, StudentUniqueId or EducationOrganizationId.
        /// </summary>
        [MaxLength(100)]
        public string ResourceId { get; set; }

        /// <summary>
        /// This is the resource associated with the validation rule. This is denormalized from the validation rule, every instance of a given RuleId will have the same Ed-Fi resource
        /// </summary>
        [MaxLength(128)]
        public string ResourceType { get; set; }

        /// <summary>
        /// Along with NameSpace, This is useful for limiting what systems can consume the validation results and routing the validation results within the consuming system. 
        /// </summary>
        [Required]
        public EducationOrganizationReference EducationOrganizationReference { get; set; }

        /// <summary>
        /// Reference back to an EdFi student object, when applicable for that validation
        /// </summary>
        public StudentReference StudentReference { get; set; }

        /// <summary>
        /// Reference back to an EdFi staff object, when applicable for that validation
        /// </summary>
        public StaffReference StaffReference { get; set; }

        /// <summary>
        /// Along with EducationOrganization, this can be used for limiting what systems can consume the validation results and routing the validation results within the consuming system.
        /// </summary>
        [MaxLength(255)]
        public string Namespace { get; set; }

        /// <summary>
        /// Includes the details that were used in the evaluation of the validation rule.
        /// </summary>
        public AdditionalContext[] AdditionalContext { get; set; }

    }

    public class AdditionalContext
    {
        /// <summary>
        /// Custom data element which provides additional context for the result.
        /// </summary>
        [MaxLength(1024)]
        public string IdentificationCode { get; set; }

        /// <summary>
        /// Additional context for the result.
        /// </summary>
        [MaxLength(4000)]
        public string CustomizationValue { get; set; }
    }
}
