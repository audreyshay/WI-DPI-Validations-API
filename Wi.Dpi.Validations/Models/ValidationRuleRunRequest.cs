namespace Wi.Dpi.Validations.Models
{
    public class ValidationRuleRunRequest
    {
        public int? Offset { get; set; }
        public int? Limit { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// The unique Id for a run
        /// </summary>
        public string RunIdentifier { get; set; }
        /// <summary>
        /// The name of the Host or ODS that was evaluated in a run
        /// </summary>
        public string Host { get; set; }
        /// <summary>
        /// A reference to the validation engine that was responsible for this run
        /// </summary>
        public string ValidationEngine { get; set; }
       
    }
}
