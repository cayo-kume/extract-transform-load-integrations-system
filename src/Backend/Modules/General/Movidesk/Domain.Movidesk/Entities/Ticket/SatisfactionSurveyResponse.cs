using Domain.Core.Extensions;

namespace Domain.Movidesk.Entities.Ticket
{
    public class SatisfactionSurveyResponse
    {
        public string ticketId { get; private set; }
        public string comments { get; private set; }
        public string? responsedBy { get; private set; } // Person
        
        public DateTime responseDate { get; private set; }
        
        public int id { get; private set; }
        public int satisfactionSurveyModel { get; private set; }
        public int satisfactionSurveyNetPromoterScoreResponse { get; private set; }
        public int satisfactionSurveyPositiveNegativeResponse { get; private set; }
        public int satisfactionSurveySmileyFacesResponse { get; private set; }

        public SatisfactionSurveyResponse() { }

        public SatisfactionSurveyResponse(Dtos.Tickets.SatisfactionSurveyResponse satisfactionSurveyResponseDto, string ticketId)
        {
            this.ticketId = ticketId;
            comments = satisfactionSurveyResponseDto.comments;
            responsedBy = satisfactionSurveyResponseDto.responsedBy?.id;

            responseDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(satisfactionSurveyResponseDto.responseDate);

            id = ConverterValidatorsExtensions.ConvertToInt32Validation(satisfactionSurveyResponseDto.id);
            satisfactionSurveyModel = ConverterValidatorsExtensions.ConvertToInt32Validation(satisfactionSurveyResponseDto.satisfactionSurveyModel);
            satisfactionSurveyNetPromoterScoreResponse = ConverterValidatorsExtensions.ConvertToInt32Validation(satisfactionSurveyResponseDto.satisfactionSurveyNetPromoterScoreResponse);
            satisfactionSurveyPositiveNegativeResponse = ConverterValidatorsExtensions.ConvertToInt32Validation(satisfactionSurveyResponseDto.satisfactionSurveyPositiveNegativeResponse);
            satisfactionSurveySmileyFacesResponse = ConverterValidatorsExtensions.ConvertToInt32Validation(satisfactionSurveyResponseDto.satisfactionSurveySmileyFacesResponse);
        }
    }
}
