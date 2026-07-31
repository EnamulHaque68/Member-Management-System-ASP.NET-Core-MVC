using CoreIdentityWithOWIN.Models;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using CoreIdentityWithOWIN.DTOS;

namespace CoreIdentityWithOWIN.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private readonly AppDbContext _db;

        public MemberRepository(AppDbContext db)
        {
            _db = db;
        }

        public Member AddMember(Member member)
        {
            try
            {
               
                var memberType = _db.MemberTypes.Find(member.TypeId);
                if (memberType == null)
                {
                    throw new Exception($"MemberType with ID {member.TypeId} does not exist");
                }

                _db.Members.Add(member);
                _db.SaveChanges();
                return member;
            }
            catch (DbUpdateException ex)
            {
                throw new Exception($"Database error: {ex.InnerException?.Message ?? ex.Message}");
            }
        }

        public void AddTransactionByMemberId(int id, List<Transaction> transactions)
        {
            if (transactions != null && transactions.Any())
            {
                foreach (var transaction in transactions)
                {
                    transaction.MemberId = id;
                    _db.Transactions.Add(transaction);
                }
                _db.SaveChanges();
            }
        }

        public Member DeleteStudentByMemberId(int id)
        {
            var member = _db.Members.Find(id);
            if (member != null)
            {
                _db.Members.Remove(member);
                _db.SaveChanges();
            }
            return member;
        }

        public void DeleteTransactionByMemberId(int id)
        {
            var modules = _db.Transactions.Where(m => m.MemberId == id).ToList();
            if (modules != null && modules.Any())
            {
                _db.Transactions.RemoveRange(modules);
                _db.SaveChanges();
            }
        }

        public IEnumerable<Member> GetMembers()
        {
            return _db.Members
                .Include(s => s.MemberType)
                .Include(s => s.Transactions)
                .OrderByDescending(s => s.MemberId)
                .ToList();
        }

        public IEnumerable<MemberType> GetMemberTypes()
        {
            return _db.MemberTypes.ToList();
        }

        public Member GetStudentByMemberId(int id)
        {
            return _db.Members
                .Include(a => a.Transactions)
                .FirstOrDefault(x => x.MemberId == id);
        }

        public Member UpdateMember(Member member)
        {
            _db.Entry(member).State = EntityState.Modified;
            _db.SaveChanges();
            return member;
        }
    }
}