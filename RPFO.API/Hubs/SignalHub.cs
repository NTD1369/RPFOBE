using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
 
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using RPFO.API.Helpers;
using RPFO.Application.Interfaces;
using RPFO.Utilities.Constants;

namespace RPFO.API.Hubs
{

    [Authorize]
    public class SignaRHub : Hub
    {
        private static List<string> users = new List<string>();
        private IResponseCacheService cacheService;
        public SignaRHub(  IResponseCacheService responseCacheService/*, IHubContext<RequestHub> hubContext*/
         )//: base(hubContext)
        {
           
            this.cacheService = responseCacheService;
        }
        public void GetDataFromClient(string userId, string connectionId)
        {
            Clients.Client(connectionId).SendAsync("clientMethodName", $"Updated userid {userId}");
        }
        public void ResponseDataFromClient(string userId, string CounterId,  string connectionId)
        {
            var userList = AppConstants.ActiveUsers.Where(x => x.UserId == userId).ToList();
            foreach(var user in userList)
            {
                user.Status = "Online";
                user.CounterId = CounterId;
            }    
            
        }
        public async Task<bool>  checkStatusX(List<ActiveUser> users, string userId, string counterId)
        {
            var result = false;
            int count = 1;
            int delayTime = 5000;
            foreach (var user in users)
            {
                //result = await checkUserStatusZ(user.UserId, user.CounterId, user.ConnectionId);
                //var checkUser = AppConstants.ActiveUsers.Where(x => x.UserId == user.UserId).FirstOrDefault();
                //if(checkUser!= null && checkUser.Status == "Online")
                //{
                //    foreach (var userinList in AppConstants.ActiveUsers.Where(x => x.UserId == user.UserId))
                //    {
                //        userinList.Status = "Offline";
                //    }
                //    return true;
                //}    
                await  Clients.Client(user.ConnectionId).SendAsync("CheckStatusUser", user.ConnectionId + " " + user.UserId);
                count++;
            }
            if(count > 10)
            {
                delayTime = (count * 1000) / 2;
            }    
           
            await Task.Delay(delayTime);

            var checkUser = AppConstants.ActiveUsers.Where(x => x.UserId == userId).FirstOrDefault();
            if (checkUser != null && checkUser.Status == "Online" && checkUser.CounterId != counterId)
            {

                //var userRemove = AppConstants.ActiveUsers.Where(x => x.UserId == userId).ToList();
                //foreach (var user in userRemove)
                //{
                //    AppConstants.ActiveUsers.Remove(user);
                //}
 
                return true;
            }
            else
            {
                var userRemove = AppConstants.ActiveUsers.Where(x => x.UserId == userId).ToList();
                foreach (var user in userRemove)
                {
                    AppConstants.ActiveUsers.Remove(user);
                }

            }
            return result;
        }

        //public async Task<bool> checkUserStatusZ(string UserId, string CounterId,  string ConnectionId)
        //{
        //    await Clients.Client(ConnectionId).SendAsync("CheckStatusUser", ConnectionId + " " + UserId);
        //    await Task.Delay(5000);
        //    var checkUser = AppConstants.ActiveUsers.Where(x => x.UserId == UserId).FirstOrDefault();
        //    if (checkUser != null && checkUser.Status == "Online")
        //    {
        //        var userRemove = AppConstants.ActiveUsers.Where(x => x.UserId == UserId).ToList();
        //        foreach(var user in userRemove)
        //        {
        //            AppConstants.ActiveUsers.Remove(user);
        //        }    
                
        //        //foreach (var userinList in )
        //        //{
        //        //    userinList.Status = "Offline";
        //        //}
        //        return true;
        //    } 
        //    return false;
        //}
        public async Task<bool>  CheckUserStatus(string CompanyCode, string UserName, string CounterId)
        {
            var result = false;


            if(AppConstants.ActiveUsers != null && AppConstants.ActiveUsers.Count > 0)
            {
                var httpContext = Context.GetHttpContext();
                if (httpContext != null)
                {
                    var type = httpContext.Request.Query["Type"];
                    if (type == "login")
                    {

                        var userList = AppConstants.ActiveUsers.Where(x => x.UserId == UserName).ToList();
                        if (userList != null && userList.Count() > 0)
                        {
                            result = await checkStatusX(userList, UserName, CounterId) ;

                            //foreach (var conectId in userList)
                            //{
                            //    Clients.Client(conectId.ConnectionId).SendAsync("CheckStatusUser", conectId.ConnectionId + " " + UserName);
                            //}    
                           
                            //return true;
                        }
                    }

                }
            }
            //Context.

            return  result ;
        }
        private string PrefixCacheGetItem = "QAUser-{0}-{1}";
        //[Cached(600)]
        public override async Task OnConnectedAsync()
        { 
            var connectionId = Context.ConnectionId;
            //Context.
            var httpContext = Context.GetHttpContext();
            if (httpContext != null)
            {
                var type = httpContext.Request.Query["Type"];
                var counterId = httpContext.Request.Query["CounterId"];
               
                if (type == "login")
                {
                   
                    ActiveUser user = new ActiveUser();
                    user.CompanyCode = "CP001";
                    user.ConnectionId = connectionId; 
                    user.UserId = Context.User.Identity.Name;
                    user.CounterId = counterId.ToString();
                     
                    string keyCache = string.Format(PrefixCacheGetItem, $"{counterId}", user.UserId);
                    //string storeCache = cacheService.GetCachedData<string>(keyCache);
                    var checkData = cacheService.GetCachedData<string>(keyCache);
                    var check = false;
                    if (string.IsNullOrEmpty(checkData))
                    {

                        check = await CheckUserStatus("CP001", user.UserId, counterId);
                        TimeSpan timeQuickAction = TimeSpan.FromSeconds(900);
                        cacheService.CacheData<string>( check.ToString(), keyCache, timeQuickAction);
                    }
                    else
                    {
                        check = bool.Parse(checkData);
                    }    
                    //else
                    //{
                       

                    //}

                    //var check = await CheckUserStatus("CP001", user.UserId, counterId);
                    //if (string.IsNullOrEmpty(storeCache))
                    //{
                    //    cacheService.CacheData<string>(keyCache, keyCache, timeQuickAction);
                    //}


                    //var check = await  CheckUserStatus("CP001", user.UserId, counterId) ;
                    var checkResult = check;// bool.Parse(check);
                    if(checkResult == false)
                    {
                        AppConstants.ActiveUsers.Add(user);
                        await Clients.Client(connectionId).SendAsync("WelcomeMethodName", connectionId + " " + Context.User.Identity.Name);
                         
                    }    
                    else
                    {
                        await Clients.Client(connectionId).SendAsync("OnlineMethodName", connectionId + " " + Context.User.Identity.Name); 
                         
                    }    
                    //ActiveUsers.Add(Context.User.Identity.Name);
                }
                //var jwtToken = httpContext.Request.Query["Token"];
                //var type = httpContext.Request.Query["Type"];
                //var counter = httpContext.Request.Query["Counter"];
                //var handler = new JwtSecurityTokenHandler();
                //if (!string.IsNullOrEmpty(jwtToken))
                //{
                //    var token = handler.ReadJwtToken(jwtToken);
                //    var tokenS = token as JwtSecurityToken;

                //    // replace email with your claim name
                //    var jti = tokenS.Claims.First(claim => claim.Type == "unique_name").Value;
                //    if (jti != null && jti != "")
                //    {
                //        if (type == "login")
                //        {
                //            users.Add(Context.User.Identity.Name);
                //        }
                //        if (type == "logout")
                //        {
                //            Groups.RemoveFromGroupAsync(Context.ConnectionId + "-" + jti + "-" + counter, "login");
                //        }
                //    }
                //}
            }
            ////var token = httpContext.Request.Query["Token"];
            //var group = Groups;

            //Clients.Client(connectionId).SendAsync("WelcomeMethodName", connectionId + " " + Context.User.Identity.Name);



            //return  base.OnConnectedAsync()  ;
        }
        //public void SendUserList(List<string> users)
        //{
        //    var context = connectionManager.GetHubContext<UserActivityHub>();
        //    context.Clients.All.updateUserList(users);
        //}
        public override Task OnDisconnectedAsync(Exception exception)
        {
            var connectionId = Context.ConnectionId;
            //users.Remove(Context.User.Identity.Name);

            //user.ConnectionId = connectionId;
            //user.UserId = Context.User.Identity.Name;
            var user =  AppConstants.ActiveUsers.Where(x => x.ConnectionId != connectionId).FirstOrDefault();
            AppConstants.ActiveUsers = AppConstants.ActiveUsers.Where(x => x.UserId != user.UserId).ToList();
            return base.OnDisconnectedAsync(exception);
        }
    }




}
