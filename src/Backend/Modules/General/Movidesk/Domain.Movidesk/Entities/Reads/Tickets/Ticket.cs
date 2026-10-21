using Domain.Shared.Extensions;

namespace Domain.Movidesk.Entities.Reads.Tickets
{
    public class Ticket
    {
        public DateTime integrated_or_updated_at { get; private set; }
        public string doc_company { get; private set; }
        public long id { get; private set; }
        public string protocol { get; private set; }
        public int? type { get; private set; }
        public string subject { get; private set; }
        public string category { get; private set; }
        public string urgency { get; private set; }
        public string status { get; private set; }
        public string baseStatus { get; private set; }
        public string justification { get; private set; }
        public int? origin { get; private set; }
        public DateTime? createdDate { get; private set; }
        public string originEmailAccount { get; private set; }
        public string? ownerId { get; private set; }
        public string ownerTeam { get; private set; }
        public string? createdById { get; private set; }
        public int? serviceFirstLevelId { get; private set; }
        public string serviceFirstLevel { get; private set; }
        public string serviceSecondLevel { get; private set; }
        public string serviceThirdLevel { get; private set; }
        public string contactForm { get; private set; }
        public string cc { get; private set; }
        public DateTime? resolvedIn { get; private set; }
        public DateTime? reopenedIn { get; private set; }
        public DateTime? closedIn { get; private set; }
        public DateTime? canceledIn { get; private set; }
        public bool isDeleted { get; private set; }
        public DateTime? lastActionDate { get; private set; }
        public int? actionCount { get; private set; }
        public DateTime? lastUpdate { get; private set; }
        public int? lifetimeWorkingTime { get; private set; }
        public int? stoppedTime { get; private set; }
        public int? stoppedTimeWorkingTime { get; private set; }
        public bool resolvedInFirstCall { get; private set; }
        public string chatWidget { get; private set; }
        public string chatGroup { get; private set; }
        public int? chatTalkTime { get; private set; }
        public int? chatWaitingTime { get; private set; }
        public int? sequence { get; private set; }
        public string slaAgreement { get; private set; }
        public string slaAgreementRule { get; private set; }
        public int? slaSolutionTime { get; private set; }
        public int? slaResponseTime { get; private set; }
        public bool slaSolutionChangedByUser { get; private set; }
        public string? slaSolutionChangedById { get; private set; }
        public DateTime? slaSolutionDate { get; private set; }
        public bool slaSolutionDateIsPaused { get; private set; }
        public DateTime? slaResponseDate { get; private set; }
        public DateTime? slaRealResponseDate { get; private set; }
        public string jiraIssueKey { get; private set; }
        public int? redmineIssueId { get; private set; }

        [SkipProperty]
        public List<TicketTag> tags { get; private set; } = [];

        [SkipProperty]
        public List<Client> clients { get; private set; } = [];

        [SkipProperty]
        public List<Relationship> relationships { get; private set; } = [];

        [SkipProperty]
        public List<OwnerHistory> ownerHistories { get; private set; } = [];

        [SkipProperty]
        public List<StatusHistory> statusHistories { get; private set; } = [];

        [SkipProperty]
        public List<CustomFieldValue> customFieldValues { get; private set; } = [];

        [SkipProperty]
        public List<Asset> assets { get; private set; } = [];

        [SkipProperty]
        public List<Action> actions { get; private set; } = [];

        public Ticket() { }

        public Ticket(string doc_company, DateTime integrated_or_updated_at, Dtos.Reads.Tickets.Ticket dto)
        {
            this.integrated_or_updated_at = integrated_or_updated_at;
            this.doc_company = doc_company;

            id = ConverterValidatorsExtensions.ConvertToInt64Validation(dto.id);
            protocol = ConverterValidatorsExtensions.StringLengthValidation(dto.protocol, 30);
            type = NullableConverters.ToNullableInt32(dto.type);
            subject = ConverterValidatorsExtensions.StringLengthValidation(dto.subject, 350);
            category = ConverterValidatorsExtensions.StringLengthValidation(dto.category, 128);
            urgency = ConverterValidatorsExtensions.StringLengthValidation(dto.urgency, 128);
            status = ConverterValidatorsExtensions.StringLengthValidation(dto.status, 128);
            baseStatus = ConverterValidatorsExtensions.StringLengthValidation(dto.baseStatus, 128);
            justification = ConverterValidatorsExtensions.StringLengthValidation(dto.justification, 128);
            origin = NullableConverters.ToNullableInt32(dto.origin);
            createdDate = NullableConverters.ToNullableDateTime(dto.createdDate);
            originEmailAccount = ConverterValidatorsExtensions.StringLengthValidation(dto.originEmailAccount, 128);
            ownerId = dto.owner?.id;
            ownerTeam = ConverterValidatorsExtensions.StringLengthValidation(dto.ownerTeam, 128);
            createdById = dto.createdBy?.id;
            serviceFirstLevelId = NullableConverters.ToNullableInt32(dto.serviceFirstLevelId);
            serviceFirstLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.serviceFirstLevel, 1024);
            serviceSecondLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.serviceSecondLevel, 1024);
            serviceThirdLevel = ConverterValidatorsExtensions.StringLengthValidation(dto.serviceThirdLevel, 1024);
            contactForm = ConverterValidatorsExtensions.StringLengthValidation(dto.contactForm, 128);
            cc = ConverterValidatorsExtensions.StringLengthValidation(dto.cc, 1024);
            resolvedIn = NullableConverters.ToNullableDateTime(dto.resolvedIn);
            reopenedIn = NullableConverters.ToNullableDateTime(dto.reopenedIn);
            closedIn = NullableConverters.ToNullableDateTime(dto.closedIn);
            canceledIn = NullableConverters.ToNullableDateTime(dto.canceledIn);
            isDeleted = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.isDeleted);
            lastActionDate = NullableConverters.ToNullableDateTime(dto.lastActionDate);
            actionCount = NullableConverters.ToNullableInt32(dto.actionCount);
            lastUpdate = NullableConverters.ToNullableDateTime(dto.lastUpdate);
            lifetimeWorkingTime = NullableConverters.ToNullableInt32(dto.lifeTimeWorkingTime);
            stoppedTime = NullableConverters.ToNullableInt32(dto.stoppedTime);
            stoppedTimeWorkingTime = NullableConverters.ToNullableInt32(dto.stoppedTimeWorkingTime);
            resolvedInFirstCall = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.resolvedInFirstCall);
            chatWidget = ConverterValidatorsExtensions.StringLengthValidation(dto.chatWidget, 128);
            chatGroup = ConverterValidatorsExtensions.StringLengthValidation(dto.chatGroup, 128);
            chatTalkTime = NullableConverters.ToNullableInt32(dto.chatTalkTime);
            chatWaitingTime = NullableConverters.ToNullableInt32(dto.chatWaitingTime);
            sequence = NullableConverters.ToNullableInt32(dto.sequence);
            slaAgreement = ConverterValidatorsExtensions.StringLengthValidation(dto.slaAgreement, 128);
            slaAgreementRule = ConverterValidatorsExtensions.StringLengthValidation(dto.slaAgreementRule, 128);
            slaSolutionTime = NullableConverters.ToNullableInt32(dto.slaSolutionTime);
            slaResponseTime = NullableConverters.ToNullableInt32(dto.slaResponseTime);
            slaSolutionChangedByUser = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.slaSolutionChangedByUser);
            slaSolutionChangedById = dto.slaSolutionChangedBy?.id;
            slaSolutionDate = NullableConverters.ToNullableDateTime(dto.slaSolutionDate);
            slaSolutionDateIsPaused = ConverterValidatorsExtensions.ConvertToBooleanValidation(dto.slaSolutionDateIsPaused);
            slaResponseDate = NullableConverters.ToNullableDateTime(dto.slaResponseDate);
            slaRealResponseDate = NullableConverters.ToNullableDateTime(dto.slaRealResponseDate);
            jiraIssueKey = ConverterValidatorsExtensions.StringLengthValidation(dto.jiraIssueKey, 64);
            redmineIssueId = NullableConverters.ToNullableInt32(dto.redmineIssueId);

            // tags repetidas violariam a PK (o SQL Server não diferencia maiúsculas de minúsculas)
            foreach (var tag in (dto.tags ?? []).Where(t => !String.IsNullOrWhiteSpace(t)).Distinct(StringComparer.OrdinalIgnoreCase))
                tags.Add(new TicketTag(doc_company, integrated_or_updated_at, id, tag));

            foreach (var client in (dto.clients ?? []).Where(c => !String.IsNullOrEmpty(c.id)))
                clients.Add(new Client(doc_company, integrated_or_updated_at, id, client));

            foreach (var parent in (dto.parentTickets ?? []).Where(p => !String.IsNullOrEmpty(p.id)))
                relationships.Add(new Relationship(doc_company, integrated_or_updated_at, id, "Parent", parent));

            foreach (var child in (dto.childrenTickets ?? []).Where(c => !String.IsNullOrEmpty(c.id)))
                relationships.Add(new Relationship(doc_company, integrated_or_updated_at, id, "Child", child));

            // históricos não têm id na API: numerados na ordem de changedDate (estável, o histórico só cresce)
            var ownerHistoriesOrdered = (dto.ownerHistories ?? []).OrderBy(h => NullableConverters.ToNullableDateTime(h.changedDate)).ToList();
            for (int i = 0; i < ownerHistoriesOrdered.Count; i++)
                ownerHistories.Add(new OwnerHistory(doc_company, integrated_or_updated_at, id, i + 1, ownerHistoriesOrdered[i]));

            var statusHistoriesOrdered = (dto.statusHistories ?? []).OrderBy(h => NullableConverters.ToNullableDateTime(h.changedDate)).ToList();
            for (int i = 0; i < statusHistoriesOrdered.Count; i++)
                statusHistories.Add(new StatusHistory(doc_company, integrated_or_updated_at, id, i + 1, statusHistoriesOrdered[i]));

            foreach (var customFieldValue in dto.customFieldValues ?? [])
                customFieldValues.Add(new CustomFieldValue(doc_company, integrated_or_updated_at, id, customFieldValue));

            foreach (var asset in (dto.assets ?? []).Where(a => !String.IsNullOrEmpty(a.id)))
                assets.Add(new Asset(doc_company, integrated_or_updated_at, id, asset));

            foreach (var action in dto.actions ?? [])
                actions.Add(new Action(doc_company, integrated_or_updated_at, id, action));
        }
    }
}
