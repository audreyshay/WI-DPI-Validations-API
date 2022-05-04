namespace Wi.Dpi.Validations.Models
{
    public class ValidationRuleRequest
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// The unique Id for a validation rule.
        /// </summary>
        public string RuleIdentifier { get; set; }
        /// <summary>
        /// The source or origin of the rule.
        /// </summary>
        public string RuleSource { get; set; }
        /// <summary>
        /// The current status of if the rule. Examples are 'Active', 'Under Analysis', 'Inactive', 'Deprecated'.
        /// </summary>
        public string RuleStatus { get; set; }
        /// <summary>
        ///  A category for the type of validation rule.  Examples might be 'Student Demographics', 'Special Education', or 'Attendance' 
        /// </summary>
        public string Category { get; set; }
        /// <summary>
        ///  Whether the validation rule is a 'Warning', 'Minor Validation Error', 'Major Validation Error' or other value standardized by the API
        /// </summary>
        public string SeverityDescriptor { get; set; }
        /// <summary>
        /// Refers back to a unique identifier for this rule in another system (such as a state-maintained repository of validation rules)
        /// </summary>
        public string ExternalRuleId { get; set; }
    }
}
