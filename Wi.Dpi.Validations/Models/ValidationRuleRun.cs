using EdFi.Ods.Api.Common.Models.Resources.EducationOrganization.EdFi;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Wi.Dpi.Validations.Models
{
    /// <summary>
    /// This element will track the runs of the validation rules. 
    /// </summary>
    public class ValidationRuleRun
    {
        public string Id { get; set; }

        /// <summary>
        /// This is a unique Id for each run
        /// </summary>
        [Required]
        [MaxLength(100)]
        public string RunIdentifier { get; set; }

        /// <summary>
        /// This is time that the validation run was started.
        /// </summary>
        [Required]
        public DateTime RunStartDateTime { get; set; }

        /// <summary>
        /// This is the time the validation run finished.
        /// </summary>
        public DateTime? RunFinishDateTime { get; set; }

        /// <summary>
        /// This will denote the status of the validation run.  Possible values include 'Running','Finished','Stopped-manual','Stopped-Error'
        /// </summary>
        [Required]
        [MaxLength(306)]
        public string RunStatusDescriptor { get; set; }

        /// <summary>
        /// The name of the Host or ODS that was evaluated in this run
        /// </summary>
        [MaxLength(100)]
        public string Host { get; set; }

        /// <summary>
        /// A reference to the validation engine that was responsible for this run
        /// </summary>
        [MaxLength(100)]
         public string ValidationEngine { get; set; }

        /// <summary>
        /// Organization for which the run was executed. 
        /// </summary>
        public EducationOrganizationReference EducationOrganizationReference { get; set; }
    }

}
