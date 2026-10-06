using Domain.Core.Extensions;
using Domain.Movidesk.Entities.Person;

namespace Domain.Movidesk.Entities.Ticket
{
    public class Ticket
    {
        public string id { get; private set; }
		public string protocol { get; private set; }
		public string subject { get; private set; }
		public string category { get; private set; }
		public string urgency { get; private set; }				
		public string status { get; private set; }
		public string baseStatus { get; private set; }
		public string justification { get; private set; }
		public string originEmailAccount { get; private set; }
		public string? owner { get; private set; } //Person
		public string ownerTeam { get; private set; }
		public string? createdBy { get; private set; } //Person
		public string serviceFirstLevel { get; private set; }	
		public string serviceSecondLevel { get; private set; }	
		public string serviceThirdLevel { get; private set; }	
		public string contactForm { get; private set; }			
		public string cc { get; private set; }					
		public string chatWidget { get; private set; }			
		public string chatGroup { get; private set; }			
		public string slaAgreement { get; private set; }		
		public string slaAgreementRule { get; private set; }	
		public string? slaSolutionChangedBy { get; private set; } //Person
		public string jiraIssueKey { get; private set; }
		
		public DateTime createdDate { get; private set; }
		public DateTime resolvedIn { get; private set; }		
		public DateTime reopenedIn { get; private set; }			
		public DateTime canceledIn { get; private set; }		
		public DateTime closedIn { get; private set; }			
		public DateTime lastActionDate { get; private set; }	
		public DateTime lastUpdate { get; private set; }		
		public DateTime slaSolutionDate { get; private set; }	
		public DateTime slaResponseDate { get; private set; }	
		public DateTime slaRealResponseDate { get; private set; }
		
		public int redmineIssueId { get; private set; }			
		public int type { get; private set; }
		public int origin { get; private set; }
		public int serviceFirstLevelId { get; private set; }
		public int actionCount { get; private set; }
		public int lifetimeWorkingTime { get; private set; }
		public int stoppedTime { get; private set; }
		public int stoppedTimeWorkingTime { get; private set; }
		public int chatTalkTime { get; private set; }
		public int chatWaitingTime { get; private set; }
		public int sequence { get; private set; }
		public int slaSolutionTime { get; private set; }
		public int slaResponseTime { get; private set; }
		
		public bool slaSolutionChangedByUser { get; private set; }
		public bool isDeleted { get; private set; }				
		public bool resolvedInFirstCall { get; private set; }	
		public bool slaSolutionDateIsPaused { get; private set; }

        [SkipProperty]
        public List<TicketServiceFull> serviceFull { get; private set; } = [];

        [SkipProperty]
        public List<TicketTag> tags { get; private set; } = [];
        
		[SkipProperty]
		public List<SatisfactionSurveyResponse> satisfactionSurveyResponsese { get; private set; } = [];
        
		[SkipProperty]
		public List<CustomFieldValue> customFieldValues { get; private set; } = [];
        
		[SkipProperty]
		public List<Asset> assets { get; private set; } = [];
        
		[SkipProperty]
		public List<ParentChildTicket> childrenTickets { get; private set; } = [];
        
		[SkipProperty]
		public List<ParentChildTicket> parentTickets { get; private set; } = [];
        
		[SkipProperty]
		public List<Client> clients { get; private set; } = [];
        
		[SkipProperty]
		public List<Action> actions { get; private set; } = [];
        
		[SkipProperty]
		public List<OwnerHistory> ownerHistories { get; private set; } = [];

		[SkipProperty]
		public List<StatusHistory> statusHistories { get; private set; } = [];

        public Ticket() { }

        public Ticket(Dtos.Tickets.Ticket ticketDto)
        {
            id = ticketDto.id;
			protocol = ticketDto.protocol;
			subject = ticketDto.subject;
			category = ticketDto.category;
			urgency = ticketDto.urgency;
			status = ticketDto.status;
			baseStatus = ticketDto.baseStatus;
			justification = ticketDto.justification;
			originEmailAccount = ticketDto.originEmailAccount;
			owner = ticketDto.owner?.id;
			ownerTeam = ticketDto.ownerTeam;
			createdBy = ticketDto.createdBy?.id; 
			serviceFirstLevel = ticketDto.serviceFirstLevel;
			serviceSecondLevel = ticketDto.serviceSecondLevel;
			serviceThirdLevel = ticketDto.serviceThirdLevel;
			contactForm = ticketDto.contactForm;
			cc = ticketDto.cc;
			chatWidget = ticketDto.chatWidget;
			chatGroup = ticketDto.chatGroup;
			slaAgreement = ticketDto.slaAgreement;
			slaAgreementRule = ticketDto.slaAgreementRule;
			slaSolutionChangedBy = ticketDto.slaSolutionChangedBy?.id;
			jiraIssueKey = ticketDto.jiraIssueKey;

            createdDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.createdDate);
			resolvedIn = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.resolvedIn);
            reopenedIn = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.reopenedIn);
			canceledIn = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.canceledIn);
			closedIn = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.closedIn);
			lastActionDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.lastActionDate);
			lastUpdate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.lastUpdate);
			slaSolutionDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.slaSolutionDate);
			slaResponseDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.slaResponseDate);
			slaRealResponseDate = ConverterValidatorsExtensions.ConvertToDateTimeValidation(ticketDto.slaRealResponseDate);

			redmineIssueId = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.redmineIssueId);
			type = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.type);
			origin = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.origin);
			serviceFirstLevelId = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.serviceFirstLevelId);
			actionCount = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.actionCount);
			lifetimeWorkingTime = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.lifeTimeWorkingTime);
			stoppedTime = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.stoppedTime);
			stoppedTimeWorkingTime = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.stoppedTimeWorkingTime);
			chatTalkTime = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.chatTalkTime);
			chatWaitingTime = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.chatWaitingTime);
			sequence = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.sequence);
			slaSolutionTime = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.slaSolutionTime);
			slaResponseTime = ConverterValidatorsExtensions.ConvertToInt32Validation(ticketDto.slaResponsetime);

            slaSolutionChangedByUser = ConverterValidatorsExtensions.ConvertToBooleanValidation(ticketDto.slaSolutionChangedByUser);
			isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(ticketDto.isDeleted);
			resolvedInFirstCall = ConverterValidatorsExtensions.ConvertToBooleanValidation(ticketDto.resolvedInFirstCall);
			slaSolutionDateIsPaused = ConverterValidatorsExtensions.ConvertToBooleanValidation(ticketDto.slaSolutionDateIsPaused);

            for (int i = 0; i <= ticketDto.serviceFull?.Count(); i++)
            {
                serviceFull.Add(new TicketServiceFull(ticketDto.id, i + 1, ticketDto.serviceFull[i]));
            }

            for (int i = 0; i <= ticketDto.tags?.Count(); i++)
            {
                tags.Add(new TicketTag(ticketDto.id, i + 1, ticketDto.tags[i]));
            }

            ticketDto.satisfactionSurveyResponsese?.ForEach(response =>
                satisfactionSurveyResponsese.Add(new SatisfactionSurveyResponse(response, ticketDto.id)
            ));

            ticketDto.customFieldValues?.ForEach(customFieldValue =>
                customFieldValues.Add(new CustomFieldValue(customFieldValue, ticketDto.id)
            ));

            ticketDto.emails?.ForEach(email =>
                emails.Add(new Email(personDto.id, email)
            ));

            ticketDto.relationships?.ForEach(relationship =>
                relationships.Add(new Relationship(personDto.id, relationship)
            ));

            ticketDto.customFieldValues?.ForEach(customFieldValue =>
                customFieldValues.Add(new CustomFieldValue(personDto.id, customFieldValue)
            ));

            ticketDto.atAssets?.ForEach(atAsset =>
                atAssets.Add(new Asset(personDto.id, atAsset)
            ));
        }
    }
}
