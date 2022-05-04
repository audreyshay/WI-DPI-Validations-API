using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Wi.Dpi.Validations.Models
{

    public class RunStatusDescriptor
    {
        public string Id { get; set; }

        /// <summary>
        /// A unique identifier used as Primary Key, not derived from business logic, when acting as Foreign Key, references the parent table.
        /// </summary>
        [Required]
        public int RunStatusDescriptorId { get; set; }

        /// <summary>
        /// A code or abbreviation that is used to refer to the descriptor.
        /// </summary>
        [Required]
        [MaxLength(50)]
        public string CodeValue { get; set; }

        /// <summary>
        /// The description of the descriptor.
        /// </summary>
        [MaxLength(1024)]
        public string Description { get; set; }

        /// <summary>
        /// The beginning date of the period when the descriptor is in effect. If omitted, the default is immediate effectiveness.
        /// </summary>
        public DateTime? EffectiveBeginDate { get; set; }

        /// <summary>
        /// The end date of the period when the descriptor is in effect.
        /// </summary>
        public DateTime? EffectiveEndDate { get; set; }

        /// <summary>
        /// A globally unique namespace that identifies this descriptor set. Author is strongly encouraged to use the Universal Resource Identifier (http, ftp, file, etc.) for the source of the descriptor definition. Best practice is for this source to be the descriptor file itself, so that it can be machine-readable and be fetched in real-time, if necessary.
        /// </summary>
        [Required]
        [MaxLength(255)]
        public string Namespace { get; set; }

        /// <summary>
        /// A unique identifier used as Primary Key, not derived from business logic, when acting as Foreign Key, references the parent table.
        /// </summary>
        public int? PriorDescriptorId { get; set; }

        /// <summary>
        /// A shortened description for the descriptor.
        /// </summary>
        [Required]
        [MaxLength(75)]
        public string ShortDescription { get; set; }
    }
}
