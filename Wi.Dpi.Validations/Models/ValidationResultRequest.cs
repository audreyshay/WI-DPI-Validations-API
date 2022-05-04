namespace Wi.Dpi.Validations.Models
{
    public class ValidationResultRequest
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string Id { get; set; }
       
        /// <summary>
        /// The unique id for a validation result.
        /// </summary>
        public string ResultIdentifier { get; set; }
        /// <summary>
        /// The unique Id for the run
        /// </summary>
        public string RunIdentifier { get; set; }
        /// <summary>
        /// The the unique Id for the validation rule.
        /// </summary>
        public string RuleIdentifier { get; set; }
        /// <summary>
        /// The source or origin of the rule.
        /// </summary>
        public string RuleSource { get; set; }
        /// <summary>
        /// The EducationOrganizationId for the organization the validation result is for.
        /// </summary>
        public int? EducationOrganizationId { get; set; }
        /// <summary>
        /// The StudentUniqueId for the student the validation result is related to.
        /// </summary>
        public string StudentUniqueId { get; set; }
        /// <summary>
        /// The StaffUniqueId for the staff the validation result is related to.
        /// </summary>
        public string StaffUniqueId { get; set; }
        /// <summary>
        /// The Namespace the validation result is for.
        /// </summary>
        public string Namespace { get; set; }
        /// <summary>
        /// The unique identifier in the ODS that is used to reference a specific resource.
        /// </summary>
        public string ResourceId { get; set; }
        /// <summary>
        /// The resource associated with the validation rule.
        /// </summary>
        public string ResourceType { get; set; }
    }
}
