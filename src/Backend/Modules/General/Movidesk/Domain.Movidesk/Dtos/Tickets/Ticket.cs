namespace Domain.Movidesk.Dtos.Tickets
{
    public class Ticket
    {
        public string id { get; init; }
        public string protocol { get; init; }
        public string type { get; init; }
        public string subject { get; init; }
        public string category { get; init; }
        public string urgency { get; init; }
        public string status { get; init; }
        public string baseStatus { get; init; }
        public string justification { get; init; }
        public string origin { get; init; }
        public string createdDate { get; init; }
        public string slaResponseDate { get; init; }
        public string originEmailAccount { get; init; }
        public string ownerTeam { get; init; }
        public string serviceFirstLevelId { get; init; }
        public string serviceFirstLevel { get; init; }
        public string serviceSecondLevel { get; init; }
        public string serviceThirdLevel { get; init; }
        public string contactForm { get; init; }
        public string cc { get; init; }
        public string resolvedIn { get; init; }
        public string reopenedIn { get; init; }
        public string closedIn { get; init; }
        public string lastActionDate { get; init; }
        public string actionCount { get; init; }
        public string lastUpdate { get; init; }
        public string lifeTimeWorkingTime { get; init; }
        public string stoppedTime { get; init; }
        public string stoppedTimeWorkingTime { get; init; }
        public string resolvedInFirstCall { get; init; }
        public string chatWidget { get; init; }
        public string chatGroup { get; init; }
        public string chatTalkTime { get; init; }
        public string chatWaitingTime { get; init; }
        public string sequence { get; init; }
        public string slaAgreement { get; init; }
        public string slaAgreementRule { get; init; }
        public string slaSolutionTime { get; init; }
        public string slaResponsetime { get; init; }
        public string slaSolutionChangedByUser { get; init; }
        public string slaSolutionDate { get; init; }
        public string slaSolutionDateIsPaused { get; init; }
        public string slaRealResponseDate { get; init; }
        public string jiraIssueKey { get; init; }
        public string redmineIssueId { get; init; }
        public string isDeleted { get; init; }
        public string canceledIn { get; init; }

        public Person owner { get; init; }
        public Person createdBy { get; init; }
        public Person slaSolutionChangedBy { get; init; }
        
        public List<string> serviceFull { get; init; }
        public List<string> tags { get; init; }
        public List<SatisfactionSurveyResponse> satisfactionSurveyResponsese { get; init; }
        public List<CustomFieldValue> customFieldValues { get; init; }
        public List<Asset> asinits { get; init; }
        public List<ParentChildTicket> childrenTickets { get; init; }
        public List<ParentChildTicket> parentTickets { get; init; }
        public List<Client> clients { get; init; }
        public List<Action> actions { get; init; }
        public List<OwnerHistory> ownerHistories { get; init; }
        public List<StatusHistory> statusHistories { get; init; }
    }
}
