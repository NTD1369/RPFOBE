
using AutoMapper;
using Dapper;
using RPFO.Application.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Data.Entities;
using RPFO.Data.Repositories;
using RPFO.Data.ViewModel;
using RPFO.Utilities.Dtos;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class FunctionService : IFunctionService
    {
        private readonly IGenericRepository<MFunction> _functionRepository;
        private readonly IMapper _mapper;
        public FunctionService(IGenericRepository<MFunction> functionRepository, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _functionRepository = functionRepository;
            _mapper = mapper;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(MFunction model)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("FunctionId", model.FunctionId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Name", model.Name);
                parameters.Add("Url", model.Url);
                parameters.Add("ParentId", model.ParentId);
                parameters.Add("Icon", model.Icon);
                parameters.Add("Status", model.Status);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("LicenseType", model.LicenseType);
                parameters.Add("OrderNo", model.OrderNo);
                parameters.Add("isShowMenu", model.isShowMenu);
                parameters.Add("isParent", model.isParent);
                var data = _functionRepository.Execute("USP_I_M_Function", parameters, commandType: CommandType.StoredProcedure); 
                rs.Success = true;
               
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
            throw new NotImplementedException();
        }

        public Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }
        public async Task<GenericResult> GetFunctionMenuShow(string CompanyCode)
        {
            GenericResult rs = new GenericResult();
            try
            {

                var data = await _functionRepository.GetAllAsync("USP_S_FunctionMenuShow", null, commandType: CommandType.StoredProcedure);
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;

        }


        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult rs = new GenericResult();
            try
            {

                var data = await _functionRepository.GetAllAsync($"select * from M_Function with (nolock) where CompanyCode='{CompanyCode}'", null, commandType: CommandType.Text);
                rs.Success = true;
                rs.Data = data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
        }
        public async Task<GenericResult> GetFunctionExpandAll(string CompanyCode, string userId)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var model = await _functionRepository.GetAllAsync($" [USP_S_FunctionExpandAll] '{userId}'", null, commandType: CommandType.Text);
                var rootFunctions = model.Where(c => c.isParent == true);
                int countRoot = rootFunctions.Count();
                var items = new List<MFunction>();
                foreach (var function in rootFunctions)
                {
                    //var nodeData = _mapper.Map<NodeFunctionViewModel>(function);
                    MFunction node = new MFunction();
                    node = function;
                     
                    //add the parent category to the item list
                    items.Add(node);
                    //now get all its children (separate Category in case you need recursion)
                    GetByParentIdNew(model.ToList(), node, items);
                }
                rs.Success = true;
                rs.Data = items;
                //var data = await _functionRepository.GetAllAsync("select * from M_Function with (nolock)", null, commandType: CommandType.Text);
                //return data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;

        }
        public async Task<GenericResult> GetNodeAll(string CompanyCode)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var model = await _functionRepository.GetAllAsync("select * from M_Function with (nolock)", null, commandType: CommandType.Text);
                var rootFunctions = model.Where(c => c.ParentId == null);
                int countRoot = rootFunctions.Count();
                var items = new List<NodeFunctionViewModel>();
                foreach (var function in rootFunctions)
                {
                    //var nodeData = _mapper.Map<NodeFunctionViewModel>(function);
                    NodeFunctionViewModel node = new NodeFunctionViewModel();
                    node.Data = function;

                    //add the parent category to the item list
                    items.Add(node);
                    //now get all its children (separate Category in case you need recursion)
                    GetByParentId(model.ToList(), node, items);
                }
                rs.Success = true;
                rs.Data = items;
                //var data = await _functionRepository.GetAllAsync("select * from M_Function with (nolock)", null, commandType: CommandType.Text);
                //return data;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;

        }
        #region Private Functions
        private void GetByParentId(IEnumerable<MFunction> allFunctions, NodeFunctionViewModel parent, IList<NodeFunctionViewModel> items)
        {
            var functionsEntities = allFunctions as MFunction[] ?? allFunctions.ToArray();
            var subFunctions = functionsEntities.Where(c => c.ParentId == parent.Data.FunctionId);
            foreach (var cat in subFunctions)
            {
                
                //var nodeData = _mapper.Map<NodeFunctionViewModel>(cat);
                NodeFunctionViewModel node = new NodeFunctionViewModel();
                node.Data = cat;
                parent.Children.Add(node);
                //add this category
                //items.Add(nodeData); 
                //recursive call in case your have a hierarchy more than 1 level deep
                GetByParentId(functionsEntities, node, items);
            }
        }
        private void GetByParentIdNew(IEnumerable<MFunction> allFunctions, MFunction parent, IList<MFunction> items)
        {
            var functionsEntities = allFunctions as MFunction[] ?? allFunctions.ToArray();
            if(parent.FunctionId == "Adm_Shop")
            {

            }    
            var subFunctions = functionsEntities.Where(c => c.ParentId == parent.FunctionId);
            foreach (var cat in subFunctions)
            {
                if(cat.FunctionId == "Adm_Promotion")
                {

                }    
                //var nodeData = _mapper.Map<NodeFunctionViewModel>(cat);
                MFunction node = new MFunction();
                node = cat;
                var itemcheck = parent.Items.Where(x => x.FunctionId == cat.FunctionId).FirstOrDefault();
                if (itemcheck==null)
                {
                    parent.Items.Add(node);
                }     
                //add this category
                //items.Add(nodeData); 
                //recursive call in case your have a hierarchy more than 1 level deep
                GetByParentIdNew(functionsEntities, node, items);
            }
        }
        #endregion
        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            return null;
        }

       
        public async Task<PagedList<MFunction>> GetPagedList(UserParams userParams)
        {
            try
            {
                string query = "select * from M_Function with (nolock) where 1=1 ";

                if (!string.IsNullOrEmpty(userParams.keyword))
                {
                    query += $" and FunctionId like N'%{userParams.keyword}%' or Name like N'%{userParams.keyword}%' ";
                }
                
                var data = await _functionRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.Name);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.FunctionId);
                }
                return await PagedList<MFunction>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                return null;
                throw new NotImplementedException();
            }
           
        }
        public async Task<GenericResult> UpdateMenuShow(List<MFunction> model)
        {
            GenericResult rs = new GenericResult();
            try
            {
                var parametersRM = new DynamicParameters();
                parametersRM.Add("CompanyCode", model[0].CompanyCode);
                var removeData = _functionRepository.Execute("USP_U_M_FunctionRemoveMenuShow", parametersRM, commandType: CommandType.StoredProcedure);
                foreach (var function in model)
                {
                    string query = "";
                    var parameters = new DynamicParameters();
                    parameters.Add("CompanyCode", function.CompanyCode);
                    parameters.Add("FunctionId", function.FunctionId, DbType.String);
                    parameters.Add("isShowMenu", function.isShowMenu);
                    parameters.Add("MenuOrder", function.MenuOrder); 
                    var data = _functionRepository.Execute("USP_U_M_FunctionMenuShow", parameters, commandType: CommandType.StoredProcedure);
                }    
               
                //model.UserId = Id;
                rs.Success = true;
                
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
            throw new NotImplementedException();
        }
        public async Task<GenericResult> Update(MFunction model)
        {
            GenericResult rs = new GenericResult();
            try
            { 
                string query = "";
                var parameters = new DynamicParameters();
                parameters.Add("FunctionId", model.FunctionId, DbType.String);
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("Name", model.Name);
                parameters.Add("Url", model.Url);
                parameters.Add("ParentId", model.ParentId);
                parameters.Add("Icon", model.Icon);
                parameters.Add("Status", model.Status);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("LicenseType", model.LicenseType);
                parameters.Add("OrderNo", model.OrderNo);
                parameters.Add("isShowMenu", model.isShowMenu);
                parameters.Add("isParent", model.isParent);
                var data = _functionRepository.Execute("USP_U_M_Function", parameters, commandType: CommandType.StoredProcedure);
                //model.UserId = Id;
                rs.Success = true;
                rs.Data = model;
            }
            catch (Exception ex)
            {
                rs.Success = false;
                rs.Message = ex.Message;
            }
            return rs;
            throw new NotImplementedException();
        }
    } 

}
