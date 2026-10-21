using Domain.AfterSale.Dtos.Reads.Reverses.GetReverse;
using Domain.Shared.Extensions;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.AfterSale.Validations.Reverses.GetReverse
{
    public class TrackingHistoryValidation : AbstractValidator<TrackingHistory>
    {
        public TrackingHistoryValidation()
        {
            RuleFor(x => x.id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.id));

            RuleFor(x => x.tracking_id)
                .MustBeValidInt64()
                .When(x => !string.IsNullOrEmpty(x.tracking_id));

            RuleFor(x => x.status)
                .MaximumLength(50)
                .WithMessage("O campo 'status' deve ter no máximo 50 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.status));

            RuleFor(x => x.message)
                .MaximumLength(800)
                .WithMessage("O campo 'message' deve ter no máximo 800 caracteres.")
                .When(x => !string.IsNullOrEmpty(x.message));

            RuleFor(x => x.status_updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.status_updated_at));

            RuleFor(x => x.created_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.created_at));

            RuleFor(x => x.updated_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.updated_at));

            RuleFor(x => x.deleted_at)
                .MustBeValidDateTime()
                .When(x => !string.IsNullOrEmpty(x.deleted_at));
        }
    }
}
