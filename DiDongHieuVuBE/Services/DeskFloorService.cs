using AutoMapper;
using ManageEmployee.Entities;
using ManageEmployee.Helpers;
using ManageEmployee.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace ManageEmployee.Services
{
    public interface IDeskFloorService
    {
        IEnumerable<DeskFloor> GetAll();
        DeskFLoorPagingResult GetAll(DeskFLoorPagingationRequestModel param);
        Task<string> Create(DeskFloor param);
        DeskFloor GetById(int id);
        Task<string> Update(DeskFloor param);
        string Delete(int id);
        void UpdateDeskChoose(int id, bool isChoose);
    }
    public class DeskFloorService : IDeskFloorService
    {
        private readonly ApplicationDbContext _context;

        public DeskFloorService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<DeskFloor> GetAll()
        {
            var data = _context.DeskFloors.Where(x => x.IsDeleted == false).ToList();
            return data;
        }

        public DeskFLoorPagingResult GetAll(DeskFLoorPagingationRequestModel param)
        {
            try
            {
                if (param.PageSize <= 0)
                    param.PageSize = 20;

                if (param.Page <= 0)
                    param.Page = 1;

                var listDeskFloors = (from p in _context.DeskFloors
                                      join f in _context.DeskFloors on p.FloorId equals f.Id into floor
                                      from f in floor.DefaultIfEmpty()
                                      where (string.IsNullOrEmpty(param.SearchText) ? true : p.Name.ToLower().Contains(param.SearchText.ToLower().Trim()))
                                      && p.IsDeleted == false
                                      && (param.FloorId > 0 ? p.FloorId == param.FloorId : true)
                                     select new DeskFloorModel
                                     {
                                         Id = p.Id,
                                         Name = p.Name,
                                         Description = p.Description,
                                         FloorId = p.FloorId,
                                         IsDesk = p.IsDesk,
                                         IsChoose = p.IsChoose,
                                         NumberSeat = p.NumberSeat,
                                         Position = p.Position,
                                         Code = p.Code,
                                         FloorName = f.Name,
                                         Order = f.Order,
                                     });
                if (!string.IsNullOrEmpty(param.SortField))
                {
                    switch (param.SortField)
                    {
                        case "id":
                            listDeskFloors = param.isSort ? listDeskFloors.OrderByDescending(x => x.Id) : listDeskFloors.OrderBy(x => x.Id);
                            break;
                        case "name":
                            listDeskFloors = param.isSort ? listDeskFloors.OrderByDescending(x => x.Name) : listDeskFloors.OrderBy(x => x.Name);
                            break;
                        case "code":
                            listDeskFloors = param.isSort ? listDeskFloors.OrderByDescending(x => x.Code) : listDeskFloors.OrderBy(x => x.Code);
                            break;
                    }
                }
                else
                {
                    listDeskFloors = listDeskFloors.OrderBy(x => x.Order);
                }

                // check isFloor
                listDeskFloors = (param.IsFloor == true)? listDeskFloors.Where(x => x.FloorId == 0) : listDeskFloors.Where(x => x.FloorId > 0);

                return new DeskFLoorPagingResult()
                {
                    pageIndex = param.Page,
                    PageSize = param.PageSize,
                    TotalItems = listDeskFloors.Count(),
                    DeskFloors = listDeskFloors.Skip((param.Page - 1) * param.PageSize).Take(param.PageSize).ToList()
                };
            }
            catch
            {
                return new DeskFLoorPagingResult()
                {
                    pageIndex = param.Page,
                    PageSize = param.PageSize,
                    TotalItems = 0,
                    DeskFloors = new List<DeskFloorModel>()
                };
            }
        }

        public DeskFloor GetById(int id)
        {
            try
            {
                return _context.DeskFloors.Find(id);
            }
            catch
            {
                throw new NotImplementedException();
            }
        }

        public async Task<string> Create(DeskFloor requests)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                var exist = await _context.DeskFloors.SingleOrDefaultAsync(
                    x => (x.Name == requests.Name || x.Code == requests.Code) && x.IsDesk == requests.IsDesk 
                        && x.FloorId == requests.FloorId && x.IsDeleted == false);
                if (exist != null)
                {
                    return ErrorMessages.DeskFloorNameAlreadyExist;
                }

                _context.DeskFloors.Add(requests);
                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();

                return string.Empty;
            }
            catch
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }

        public async Task<string> Update(DeskFloor requests)
        {
            try
            {
                await _context.Database.BeginTransactionAsync();
                if (checkCodeDisable(requests))
                    return ErrorMessages.FailedToDelete;

                var deskFloor = await _context.DeskFloors.SingleOrDefaultAsync(
                        x => x.Id == requests.Id && x.IsDeleted == false);
                if (deskFloor == null)
                {
                    return ErrorMessages.DataNotFound;
                }
                var exist = await _context.DeskFloors.SingleOrDefaultAsync(
                    x => (x.Name == requests.Name || x.Code == requests.Code) && x.IsDesk == requests.IsDesk 
                        && x.FloorId == requests.FloorId && x.IsDeleted == false && x.Id != requests.Id);
                if (exist != null)
                {
                    return ErrorMessages.DeskFloorNameAlreadyExist;
                }
                deskFloor.FloorId = requests.FloorId;
                deskFloor.Name = requests.Name;
                deskFloor.IsDesk = requests.IsDesk;
                deskFloor.Description = requests.Description;
                deskFloor.NumberSeat = requests.NumberSeat;
                deskFloor.Position = requests.Position;
                deskFloor.Code = requests.Code;

                _context.DeskFloors.Update(deskFloor);
                await _context.SaveChangesAsync();
                _context.Database.CommitTransaction();
                return string.Empty;
            }
            catch
            {
                _context.Database.RollbackTransaction();
                throw;
            }
        }

        public string Delete(int id)
        {
            var deskFloor = _context.DeskFloors.Find(id);
            if(checkCodeDisable(deskFloor))
                return ErrorMessages.FailedToDelete;

            if (deskFloor != null)
            {
                deskFloor.IsDeleted = true;
                _context.DeskFloors.Update(deskFloor);
                _context.SaveChanges();
            }
            return string.Empty;
        }
        public void UpdateDeskChoose(int id, bool isChoose)
        {
            var deskFloor = _context.DeskFloors.FirstOrDefault(x => x.Id == id && x.Code.ToLower() != "live");
            if (deskFloor != null)
            {
                deskFloor.IsChoose = isChoose;
                _context.DeskFloors.Update(deskFloor);
                _context.SaveChanges();
            }
        }
        public bool checkCodeDisable(DeskFloor desk)
        {
            string[] listCode = { "LIVE", "Floor"};
            if(listCode.Contains(desk.Code))
                return true;
            return false;
        }
    }
}
