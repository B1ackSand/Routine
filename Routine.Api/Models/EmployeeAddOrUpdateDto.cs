﻿using System.ComponentModel.DataAnnotations;
using Routine.Api.Entities;
using Routine.Api.ValidationAttributes;

namespace Routine.Api.Models
{
    //面向类或者成员的自定义属性输入验证
    [EmployeeNoMustDifferentFromFirstName(ErrorMessage = "员工编号必须和名不一样！！")]
    public abstract class EmployeeAddOrUpdateDto : IValidatableObject
    {
        [Display(Name = "员工号")]
        [Required(ErrorMessage = "{0}是必填项")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "{0}的长度最少是{1}")]
        public string EmployeeNo { get; set; }

        [Display(Name = "名")]
        [Required(ErrorMessage = "{0}是必填项")]
        [MaxLength(50, ErrorMessage = "{0}的长度不能超过{1}")]
        public string FirstName { get; set; }

        [Display(Name = "姓"), Required(ErrorMessage = "{0}是必填项"), MaxLength(50, ErrorMessage = "{0}的长度不能超过{1}")]
        public string LastName { get; set; }

        [Display(Name = "性别")]
        public Gender Gender { get; set; }

        [Display(Name = "出生日期")]
        public DateTime DateOfBirth { get; set; }

        //提供验证对象信息并访问,输入验证
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (FirstName == LastName)
            {
                yield return new ValidationResult("姓和名不能一样",
                    new[] { nameof(FirstName), nameof(LastName) });
            }
        }
    }
}