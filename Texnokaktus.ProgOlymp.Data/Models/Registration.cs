namespace Texnokaktus.ProgOlymp.Data.Models;

public record Registration(int Id, User User, DateTimeOffset Created, ParticipantData ParticipantData, ParentData ParentData, TeacherData TeacherData, bool PersonalDataConsent);
