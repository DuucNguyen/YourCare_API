using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using YourCare_BOs;
using YourCare_DAOs.DAOs;
using YourCare_Repos.Interfaces;

namespace YourCare_Repos.Repositories
{
    public class SpecialtyRepository : ISpecialtyRepository
    {
        private readonly SpecialtyDAO _specialtyDAO;

        public SpecialtyRepository(SpecialtyDAO specialtyDAO)
        {
            _specialtyDAO = specialtyDAO;
        }

        public async Task<bool> Add(Specialty request)
        {
            try
            {
                var find = await _specialtyDAO.GetByTitle(request.Title);
                if (find != null)
                {
                    return false;
                }
                request.IsActive = true;
                await _specialtyDAO.Create(request);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }

        public async Task<bool> Delete(string id)
        {
            try
            {
                var find = await _specialtyDAO.GetByID(id);
                if (find == null)
                {
                    return false;
                }

                await _specialtyDAO.Delete(find);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }

        public async Task<List<Specialty>> GetAll()
        {
            try
            {
                var result = await _specialtyDAO.GetAll();
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return null;
            }
        }
        public async Task<Specialty> GetByID(string id)
        {
            try
            {
                var find = await _specialtyDAO.GetByID(id);
                return find;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return null;
            }
        }

        public async Task<bool> Update(Specialty request)
        {

            try
            {
                var find = await _specialtyDAO.GetByID(request.SpecialtyID.ToString());
                if (find == null)
                {
                    return false;
                }

                find.Title = request.Title;
                find.Image = request.Image;

                await _specialtyDAO.Update(find);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message + " - " + ex.StackTrace);
                return false;
            }
        }
    }
}
