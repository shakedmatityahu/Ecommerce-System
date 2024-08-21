using MarketBackend.DAL.DTO;
using MarketBackend.Domain.Market_Client;
using MarketBackend.Domain.Models;
using MarketBackend.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketBackend.DAL
{
    public class RoleRepositoryRAM : IRoleRepository
    {
        public ConcurrentDictionary<int, ConcurrentDictionary<string, Role>> roles; //<storeId, <memberId, Role>>
        private static RoleRepositoryRAM roleRepositoryRAM = null;

        private object _lock;
        

        private RoleRepositoryRAM()
        {
            roles = new ConcurrentDictionary<int, ConcurrentDictionary<string, Role>>();
            _lock = new object();
        }

        public static RoleRepositoryRAM GetInstance()
        {
            if (roleRepositoryRAM == null)
                roleRepositoryRAM = new RoleRepositoryRAM();
            return roleRepositoryRAM;
        }

        public static void Dispose(){
            roleRepositoryRAM = new RoleRepositoryRAM();
        }

        public Role GetById(int storeId) 
        {
            if(!roles.ContainsKey(storeId))
                throw new KeyNotFoundException($"store with ID {storeId} not found.");

            return roles[storeId].Values.First(role => role.getRoleName() == RoleName.Founder);
        }

        public Role GetById(int storeId, string userName)
        {
            if (!roles.ContainsKey(storeId) && roles[storeId].ContainsKey(userName)){
                try{
                    lock (_lock)
                    {
                        RoleDTO roleDTO = DBcontext.GetInstance().Roles.Find(storeId, userName);
                        if (roleDTO != null)
                        {
                            Role role = new Role(roleDTO);
                            if (!roles.ContainsKey(storeId))
                            {
                                roles[storeId] = new ConcurrentDictionary<string, Role>();
                                roles[storeId][userName] = role;
                            }
                            else{
                                roles[storeId].TryAdd(userName, role);
                            }
                        }
                        else
                        {
                            throw new KeyNotFoundException($"member with ID {userName} at store with ID {storeId} not found.");
                        }
                    }
                }
                catch(Exception){
                throw new Exception("There was a problem in Database use- Get Role");
                }
                
            }
            return roles[storeId][userName];
        }
        public void Add(Role entity)
        {
            if (!roles.ContainsKey(entity.storeId))
            {
                roles[entity.storeId] = new ConcurrentDictionary<string, Role>();
                roles[entity.storeId][entity.userName] = entity;
            }
            else{
                roles[entity.storeId].TryAdd(entity.userName, entity);
            }
            try{
                lock (_lock)
                {
                    DBcontext.GetInstance().Roles.Add(new RoleDTO(entity));
                    DBcontext.GetInstance().SaveChanges();
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Add Role");
            }

        }
        public IEnumerable<Role> getAll()
        {
            List<RoleDTO> rolesDtos = new List<RoleDTO>();
            foreach (RoleDTO r in rolesDtos)
            {
                if (!roles.ContainsKey(r.storeId))
                {
                    roles[r.storeId] = new ConcurrentDictionary<string, Role>();
                    roles[r.storeId][r.userName] = new Role(r);
                }
                else
                {
                    roles[r.storeId].TryAdd(r.userName, new Role(r));
                }
            }

            return roles.SelectMany(store => store.Value.Values).ToList();
        }
        public void Update(Role entity)
        {
            roles[entity.storeId][entity.userName] = entity;
            try{
                lock (_lock)
                {
                    RoleDTO roleDTO = DBcontext.GetInstance().Roles.Find(entity.storeId, entity.userName);
                    if (roleDTO != null)
                    {
                        List<string> newPermissions = new List<string>();
                        foreach (Permission permission in entity.getPermissions())
                            newPermissions.Add(permission.ToString());

                        List<int> newAppointees = new List<int>();
                        foreach (Member app in entity.getAppointees())
                            newAppointees.Add(app.Id);

                        roleDTO.permissions = newPermissions;
                        roleDTO.appointees = newAppointees;
                        DBcontext.GetInstance().SaveChanges();
                    }
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Update Role");
            }
            
        }
        public void Delete(Role entity)
        {
            if (roles.ContainsKey(entity.storeId) && roles[entity.storeId].ContainsKey(entity.userName))
            {
                roles[entity.storeId].TryRemove(new KeyValuePair<string, Role>(entity.userName, entity));
            }
            try{
                lock (_lock)
                {
                    RoleDTO roleDto = DBcontext.GetInstance().Roles.Find(entity.storeId, entity.userName);
                    DBcontext.GetInstance().Roles.Remove(roleDto);
                    DBcontext.GetInstance().SaveChanges();
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Delete Role");
            }
        }

        public ConcurrentDictionary<string, Role> getShopRoles(int storeId)
        {
            if (!roles.ContainsKey(storeId))
            {
                roles[storeId] = new ConcurrentDictionary<string, Role>();
            }
            try{
                lock(_lock){
                    DBcontext.GetInstance().Roles.Where(role => role.storeId == storeId).ToList().ForEach(role => roles[storeId].TryAdd(role.userName, RoleDTO.ConvertToRole(role)));
                }
            }
            catch(Exception){
                throw new Exception("There was a problem in Database use- Get Role");
            }
            return roles[storeId];
        }
    }
}

