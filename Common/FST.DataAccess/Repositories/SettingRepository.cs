using FST.DataAccess.Entities;
using FST.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace FST.DataAccess.Repositories
{
    public class SettingRepository : BaseRepository, ISettingRepository
    {
        public SettingRepository(ApplicationDBContext context)
            : base(context) { }

        public async Task SetSettingAsync(string key, int value)
        {
            await ThreadSafeTaskExecute(async () => 
            {
                var setting = await Context.Setting.FirstOrDefaultAsync(_ => _.Key == key);
                if (setting != null)
                {
                    Context.Setting.Remove(setting);
                }
                Context.Setting.Add(new Setting()
                {
                    Key = key,
                    Value = value.ToString()
                });
                await Context.SaveChangesAsync();
            });
        }

        public async Task SetSettingAsync(string key, string value)
        {
            await ThreadSafeTaskExecute(async () =>
            {
                var setting = await Context.Setting.FirstOrDefaultAsync(_ => _.Key == key);
                if (setting != null)
                {
                    Context.Setting.Remove(setting);
                }
                Context.Setting.Add(new Setting()
                {
                    Key = key,
                    Value = value
                });
                await Context.SaveChangesAsync();
            });
        }

        public async Task<string> GetStringSettingAsync(string key)
        {
            return await ThreadSafeTaskExecute(async () =>
            {
                var setting = await Context.Setting.FirstOrDefaultAsync(_ => _.Key == key);
                return setting?.Value;
            });
        }

        public async Task<int?> GetIntSettingAsync(string key)
        {
            return await ThreadSafeTaskExecute<int?>(async () =>
            {
                var setting = await Context.Setting.FirstOrDefaultAsync(_ => _.Key == key);
                if (setting == null)
                {
                    return null;
                }
                return int.Parse(setting.Value);
            });
        }

        public int GetIntSetting(string key, int defaultValue = 0)
        {
            return ThreadSafeExecute(() =>
            {
                var setting = Context.Setting.FirstOrDefault(_ => _.Key == key);
                if (setting == null)
                {
                    return defaultValue;
                }
                return int.Parse(setting.Value);
            });
        }

        public string GetStringSetting(string key, string defaultValue = null)
        {
            return ThreadSafeExecute(() =>
            {
                var setting = Context.Setting.FirstOrDefault(_ => _.Key == key);
                if (setting == null)
                {
                    return defaultValue;
                }
                return setting?.Value;
            });
        }

        public bool GetBoolSetting(string key, bool defaultBool = false)
        {
            return ThreadSafeExecute(() =>
            {
                var setting = Context.Setting.FirstOrDefault(_ => _.Key == key);
                if (setting == null)
                {
                    return defaultBool;
                }
                return Convert.ToBoolean(int.Parse(setting.Value));
            });
        }

        public void SetSetting(string key, int value)
        {
            ThreadSafeExecute(() =>
            {
                var setting = Context.Setting.FirstOrDefault(_ => _.Key == key);
                if (setting != null)
                {
                    Context.Setting.Remove(setting);
                }
                Context.Setting.Add(new Setting()
                {
                    Key = key,
                    Value = value.ToString()
                });
                Context.SaveChanges();
            });
        }

        public void SetSetting(string key, string value)
        {
            ThreadSafeExecute(() =>
            {
                var setting = Context.Setting.FirstOrDefault(_ => _.Key == key);
                if (setting != null)
                {
                    Context.Setting.Remove(setting);
                }
                Context.Setting.Add(new Setting()
                {
                    Key = key,
                    Value = value
                });
                Context.SaveChanges();
            });
        }

        public void SetSetting(string key, bool value)
        {
            ThreadSafeExecute(() =>
            {
                var setting = Context.Setting.FirstOrDefault(_ => _.Key == key);
                if (setting != null)
                {
                    Context.Setting.Remove(setting);
                }
                Context.Setting.Add(new Setting()
                {
                    Key = key,
                    Value = value ? "1" : "0"
                });
                Context.SaveChanges();
            });
        }
    }
}