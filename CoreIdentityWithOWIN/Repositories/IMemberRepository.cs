using CoreIdentityWithOWIN.Models;
using System.Reflection;

namespace CoreIdentityWithOWIN.Repositories
{
    public interface IMemberRepository
    {
        IEnumerable<Member> GetMembers();
        Member GetStudentByMemberId(int id);
        Member UpdateMember(Member member);
        Member AddMember(Member member);
        Member DeleteStudentByMemberId(int id);
        IEnumerable<MemberType> GetMemberTypes();
        void DeleteTransactionByMemberId(int id);
        void AddTransactionByMemberId(int id, List<Transaction> transactions);
    }
}
