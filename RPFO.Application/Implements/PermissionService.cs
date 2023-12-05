
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
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPFO.Application.Implements
{
    public class PermissionService : IPermissionService
    {
        private readonly IGenericRepository<MPermission> _permissionRepository;
        //private readonly IGenericRepository<SPermission> _systempermissionRepository;
        private readonly IMapper _mapper;
        private readonly IUserService _userService;
        public PermissionService(IGenericRepository<MPermission> permissionRepository, IUserService userService, IMapper mapper/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
            _permissionRepository = permissionRepository;
            _mapper = mapper;
            _userService = userService;
            //_hubContext = hubContext;
            //_unitOfWork = unitOfWork;

        }

        public async Task<GenericResult> Create(MPermission model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("PermissionId", Guid.NewGuid());
                parameters.Add("RoleId", model.RoleId);
                parameters.Add("FunctionId", model.FunctionId);
                parameters.Add("ControlId", model.ControlId);
                parameters.Add("Permissions", model.Permissions);
                parameters.Add("CreatedBy", model.CreatedBy);
                parameters.Add("Status", model.Status); 

                var affectedRows = _permissionRepository.Insert("USP_I_M_Permission", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public Task<GenericResult> Delete(string Code)
        {
            throw new NotImplementedException();
        }

        public async Task<GenericResult> GetAll(string CompanyCode)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _permissionRepository.GetAllAsync($"select * from M_Permission with (nolock) where CompanyCode = N'{CompanyCode}'", null, commandType: CommandType.Text);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<GenericResult> GetByCode(string CompanyCode, string Code)
        {
            GenericResult result = new GenericResult();
            try
            {
                var data = await _permissionRepository.GetAsync($"select * from M_Permission with (nolock) where CompanyCode = N'{CompanyCode}' and PermissionId = '{Code}'", null, commandType: CommandType.Text);
                result.Success = true;
                result.Data = data;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        public async Task<GenericResult> CheckApproveFunctionByUser(string CompanyCode, string User, string Password, string CustomCode, string Function, string ControlId, string Permission)
        {
            GenericResult result = new GenericResult();
            try
            {
                var checkUser = await _userService.Login(User, Password, CustomCode);
                if(!checkUser.Success)
                { 
                    return checkUser;
                }    
                   
                var parameters = new DynamicParameters();
                if (!string.IsNullOrEmpty(CustomCode))
                {
                    var userGet = checkUser.Data as MUser;
                    User = userGet.Username;
                    Password = userGet.Password;
                }
                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("UserName", User);
                parameters.Add("Password", Password); 
                
                parameters.Add("FunctionId", Function);
                parameters.Add("ControlId", ControlId);
                parameters.Add("Permission", Permission);
                //using (IDbConnection db = _permissionRepository.GetConnection())
                //{

                //    if (db.State == ConnectionState.Closed)
                //        db.Open();
                //    var dataX = await db.QueryAsync<HeaderPermissionViewModel>(query, null);
                //    result.Success = true;
                //    result.Data = dataX;
                //    db.Close();
                //}
                var data = _permissionRepository.GetScalar($"USP_Check_ApprovalFunction", parameters, commandType: CommandType.StoredProcedure);
                if(data.ToLower() == "true" || data.ToLower() == "1")
                {
                    result.Success = true;
                    result.Message = "";
                }   
                else
                {
                    result.Success = false;
                    result.Message = "User " + User + " does not have permission to perform this operation, please login with another user";
                }    
              
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }


        public async Task<GenericResult> CheckFunctionByUserName(string CompanyCode, string User, string Function, string ControlId, string Permission)
        {
            GenericResult result = new GenericResult();
            try
            {
                //var checkUser = await _userService.Login(User, Password);
                //if (!checkUser.Success)
                //{
                //    return checkUser;
                //}

                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", CompanyCode);
                parameters.Add("UserName", User);
                parameters.Add("Password", "");
                parameters.Add("FunctionId", Function);
                parameters.Add("ControlId", ControlId);
                parameters.Add("Permission", Permission);
                //using (IDbConnection db = _permissionRepository.GetConnection())
                //{

                //    if (db.State == ConnectionState.Closed)
                //        db.Open();
                //    var dataX = await db.QueryAsync<HeaderPermissionViewModel>(query, null);
                //    result.Success = true;
                //    result.Data = dataX;
                //    db.Close();
                //}
                var data = _permissionRepository.GetScalar($"USP_Check_ApprovalFunction", parameters, commandType: CommandType.StoredProcedure);
                if (data.ToLower() == "true" || data.ToLower() == "1")
                {
                    result.Success = true;
                    result.Message = "";
                }
                else
                {
                    result.Success = false;
                    result.Message = "User " + User + " does not have permission to perform this operation, please login with another user";
                }

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        public async Task<PagedList<MPermission>> GetPagedList(UserParams userParams)
        {
            try
            {
                var data = await _permissionRepository.GetAllAsync($"select * from M_Permission with (nolock) where Permissions like N'%{userParams.keyword}%' or ControlId like N'%{userParams.keyword}%' or FunctionId like N'%{userParams.keyword}%' ", null, commandType: CommandType.Text);
                //var mock = data.AsQueryable().BuildMock();
                if (userParams.OrderBy == "byName")
                {
                    data.OrderByDescending(x => x.Permissions);
                }
                if (userParams.OrderBy == "byId")
                {
                    data.OrderByDescending(x => x.PermissionId);
                }
                return await PagedList<MPermission>.Create(data, userParams.PageNumber, userParams.PageSize);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public async Task<GenericResult> GetHeaderPermission()
        {

            GenericResult result = new GenericResult();
            try
            {
                string query = "select * from fn_GetHeaderPermission ()";
             
                using (IDbConnection db = _permissionRepository.GetConnection())
                {

                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var dataX = await db.QueryAsync<HeaderPermissionViewModel>(query, null);
                    result.Success = true;
                    result.Data = dataX;
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetHeaderFunctionPermission()
        {

            GenericResult result = new GenericResult();
            try
            {
                string query = "select * from fn_GetHeaderFunctionPermission()";

                using (IDbConnection db = _permissionRepository.GetConnection())
                {

                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var dataX = await db.QueryAsync<HeaderPermissionViewModel>(query, null);
                    result.Success = true;
                    result.Data = dataX;
                    db.Close();
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        public async Task<GenericResult> GetPermissionByFunction(string Function)
        {

            GenericResult result = new GenericResult();
            try
            {
                string query = "[USP_GetPermissionList] ''";
 

                var data = await _permissionRepository.GetAllAsync(query, null, commandType: CommandType.Text);
                result.Success = true;
                result.Data = data;
               
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                
            }
            return result;
        }

        public async Task<GenericResult> Update(MPermission model)
        {
            GenericResult result = new GenericResult();
            try
            {
                var parameters = new DynamicParameters();

                //string key = _storeRepository.GetScalar($" select dbo.[fnc_AutoGenShift] ('{model.StoreId}')", null, commandType: CommandType.Text);
                //model.ShiftId = key;
                parameters.Add("CompanyCode", model.CompanyCode);
                parameters.Add("PermissionId", model.PermissionId); 
                parameters.Add("RoleId", model.RoleId);
                parameters.Add("FunctionId", model.FunctionId);
                parameters.Add("ControlId", model.ControlId);
                parameters.Add("Permissions", model.Permissions);
                parameters.Add("ModifiedBy", model.ModifiedBy);
                parameters.Add("Status", model.Status);
           

                var affectedRows = _permissionRepository.Update("USP_U_M_Permission", parameters, commandType: CommandType.StoredProcedure);
                result.Success = true;
                //result.Message = key;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }
        #region Private Functions
        private void GetByParentId(IEnumerable<NodeViewModel> allFunctions, NodeViewModel parent, IList<NodeViewModel> items)
        {
            var functionsEntities = allFunctions as NodeViewModel[] ?? allFunctions.ToArray();
            var subFunctions = functionsEntities.Where(c => c.Data.ParentId == parent.Data.functionId).ToList();
            foreach (var cat in subFunctions)
            {

                NodeViewModel node = new NodeViewModel();
                node = cat;
                parent.Children.Add(node);
                //add this category
                //items.Add(nodeData); 
                //recursive call in case your have a hierarchy more than 1 level deep
                GetByParentId(functionsEntities, node, items);
            }
        }
        #endregion
        public async Task<GenericResult> GetPermissionByRole(string Role)
        {
            GenericResult result = new GenericResult();
            List<NodeViewModel> lstNode = new List<NodeViewModel>();
            List<NodeViewModel> lstNew = new List<NodeViewModel>();
            try
            {
                string query = "USP_GetFunctionPermissionListByRole N'"+ Role + "'";
                using (IDbConnection db = _permissionRepository.GetConnection())
                {

                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var dataX = await db.QueryAsync(query, null);
                      
                    db.Close();
                    //foreach (var item in dataX)
                    //{
                    //    NodeViewModel node = new NodeViewModel();
                    //    node.Data = item;
                    //    lstNode.Add(node);
                    //}
                    //var rootFunctions = lstNode.Where(c => c.Data.ParentId == null);
                    //int countRoot = rootFunctions.Count();
                    
                    //foreach (var function in rootFunctions)
                    //{
                         
                    //    NodeViewModel node = new NodeViewModel();
                    //    node = function; 
                    //    //add the parent category to the item list
                    //    lstNew.Add(node);
                    //    //now get all its children (separate Category in case you need recursion)
                    //    GetByParentId(lstNode, node, lstNew);
                    //}
                  
                    //lstNode = _mapper.Map<List<NodeViewModel>>(dataX);
                    result.Success = true;
                    result.Data = dataX;
                    return result;
                }
               

            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
            return null;
        }

       
        public async Task<GenericResult> GetControlPermissionListByFunction(string Function, string RoleId)
        {
            GenericResult result = new GenericResult();
            List<NodeViewModel> lstNode = new List<NodeViewModel>();
            List<NodeViewModel> lstNew = new List<NodeViewModel>();
            try
            {
                string query = "USP_GetControlPermissionListByFunction N'"+Function+ "', N'" + RoleId + "' ";
                using (IDbConnection db = _permissionRepository.GetConnection())
                {

                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    var dataX = await db.QueryAsync(query, null);

                    db.Close();
                    //foreach (var item in dataX)
                    //{
                    //    NodeViewModel node = new NodeViewModel();
                    //    node.Data = item;
                    //    lstNode.Add(node);
                    //}
                    //var rootFunctions = lstNode.Where(c => c.Data.ParentId == null);
                    //int countRoot = rootFunctions.Count();

                    //foreach (var function in rootFunctions)
                    //{
                        
                    //    NodeViewModel node = new NodeViewModel();
                    //    node = function;
                         
                    //    lstNew.Add(node);
                       
                    //}

                    //lstNode = _mapper.Map<List<NodeViewModel>>(dataX);
                    result.Success = true;
                    result.Data = dataX;
                    return result;
                }


            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
                return result;
            }
            return null;
        }

        public async Task<GenericResult> UpdateListFunctionFermission(List<MPermission> list)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _permissionRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            foreach(var model in list)
                            {
                                //string query = $"delete M_Permission where RoleId = '{model.RoleId}'  and FunctionId = '{model.FunctionId}'  and Permissions = '{model.Permissions}'";
                                //if (!string.IsNullOrEmpty(model.ControlId))
                                //{
                                //    query += $" and ControlId = '{model.ControlId}'";
                                //}
                                DynamicParameters parametersDel = new DynamicParameters();
                                parametersDel.Add("CompanyCode", model.CompanyCode);
                                parametersDel.Add("PermissionId", model.Permissions);
                                parametersDel.Add("RoleId", model.RoleId);
                                parametersDel.Add("FunctionId", model.FunctionId);
                                parametersDel.Add("ControlId", model.ControlId);
                                //string query = $"USP_D_M_Permission N'{model.Permissions}', N'{model.CompanyCode}', N'{model.RoleId}', N'{model.FunctionId}' , N'{model.ControlId}'";
                                db.Execute("USP_D_M_Permission", parametersDel, commandType: CommandType.StoredProcedure, transaction: tran);

                                if (model.Status == "A")
                                {

                                    DynamicParameters parameters = new DynamicParameters();
                                    parameters.Add("CompanyCode", model.CompanyCode);
                                    parameters.Add("PermissionId", Guid.NewGuid());
                                    parameters.Add("RoleId", model.RoleId);
                                    parameters.Add("FunctionId", model.FunctionId);
                                    parameters.Add("ControlId", model.ControlId);
                                    parameters.Add("Permissions", model.Permissions);
                                    parameters.Add("Status", model.Status);
                                    parameters.Add("CreatedBy", model.CreatedBy);
                                    var Id = _permissionRepository.Insert("USP_I_M_Permission", parameters, commandType: CommandType.StoredProcedure);

                                }
                            }    
                           
                            result.Success = true;
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }


        }
        public async Task<GenericResult> UpdateFunctionFermission(MPermission model)
        {
            GenericResult result = new GenericResult(); 
            using (IDbConnection db = _permissionRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            //string query = $"delete M_Permission where RoleId = '{model.RoleId}'  and FunctionId = '{model.FunctionId}'  and Permissions = '{model.Permissions}'";
                            //if(!string.IsNullOrEmpty(model.ControlId))
                            //{
                            //    query += $" and ControlId = '{model.ControlId}'";
                            //}
                            //else
                            //{
                            //    query += $" and  ISNULL(ControlId, '')  = ''";
                            //}
                            //db.Execute(query, null, commandType: CommandType.Text, transaction: tran);

                            DynamicParameters parametersDel = new DynamicParameters();
                            parametersDel.Add("CompanyCode", model.CompanyCode);
                            parametersDel.Add("PermissionId", model.Permissions);
                            parametersDel.Add("RoleId", model.RoleId);
                            parametersDel.Add("FunctionId", model.FunctionId);
                            parametersDel.Add("ControlId", model.ControlId);
                            //string query = $"USP_D_M_Permission N'{model.Permissions}', N'{model.CompanyCode}', N'{model.RoleId}', N'{model.FunctionId}' , N'{model.ControlId}'";
                            db.Execute("USP_D_M_Permission", parametersDel, commandType: CommandType.StoredProcedure, transaction: tran);
                            if (model.Status=="A")
                            {

                                DynamicParameters parameters = new DynamicParameters();
                                parameters.Add("CompanyCode", model.CompanyCode);
                                parameters.Add("PermissionId", Guid.NewGuid());
                                parameters.Add("RoleId", model.RoleId);
                                parameters.Add("FunctionId", model.FunctionId);
                                parameters.Add("ControlId", model.ControlId);
                                parameters.Add("Permissions", model.Permissions);
                                parameters.Add("Status", model.Status);
                                parameters.Add("CreatedBy", model.CreatedBy);
                                var Id = _permissionRepository.Insert("USP_I_M_Permission", parameters, commandType: CommandType.StoredProcedure);
                                
                            } 
                            result.Success = true;
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }

            
        }

        public async Task<GenericResult> GetFunctionPermissionByUser(string UserName)
        {
            GenericResult result = new GenericResult();
            using (IDbConnection db = _permissionRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            string query = "USP_GetPermissionUser N'" + UserName + "'";
                            var dataX = await db.QueryAsync(query, null, tran);
                            
                            db.Close();
                            result.Success = true;
                            result.Data = dataX;
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }

        public GenericResult CopyFromRole(string CompanyCode, string FrRole,string ToRole, string By)
        { 
            GenericResult result = new GenericResult();
            using (IDbConnection db = _permissionRepository.GetConnection())
            {
                try
                {
                    if (db.State == ConnectionState.Closed)
                        db.Open();
                    using (var tran = db.BeginTransaction())
                    {
                        try
                        {
                            string query = $"[USP_CopyPermision] N'{CompanyCode}', N'{FrRole}', N'{ToRole}', N'{By}'";
                            var dataX = db.Execute(query, null, tran);
                            tran.Commit();
                            db.Close();
                            result.Success = true;
                            
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            throw ex;
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                    result.Success = false;
                    result.Message = ex.Message;
                    //throw ex;
                }
                finally
                {
                    if (db.State == ConnectionState.Open)
                        db.Close();
                }
                return result;
            }
        }
    } 

}
