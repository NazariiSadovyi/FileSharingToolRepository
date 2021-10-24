using FST.DataAccess.Entities;
using FST.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace FST.DataAccess.Repositories
{
    public class SettingRepository : ISettingRepository
    {
        private static object _locker = new object();

        private readonly ApplicationDBContext _context;
        public SettingRepository(ApplicationDBContext context)
        {
            _context = context;
        }

        public async Task SetSettingAsync(string key, int value)
        {
            Monitor.Enter(_locker);
            var setting = await _context.Setting.FirstOrDefaultAsync(_ => _.Key == key);
            if (setting != null)
            {
                _context.Setting.Remove(setting);
            }
            _context.Setting.Add(new Setting()
            {
                Key = key,
                Value = value.ToString()
            });
            await _context.SaveChangesAsync();
            Monitor.Exit(_locker);
        }

        public async Task SetSettingAsync(string key, string value)
        {
            Monitor.Enter(_locker);
            var setting = await _context.Setting.FirstOrDefaultAsync(_ => _.Key == key);
            if (setting != null)
            {
                _context.Setting.Remove(setting);
            }
            _context.Setting.Add(new Setting()
            {
                Key = key,
                Value = value
            });
            await _context.SaveChangesAsync();
            Monitor.Exit(_locker);
        }

        public async Task<string> GetStringSettingAsync(string key)
        {
            Monitor.Enter(_locker);
            var setting = await _context.Setting.FirstOrDefaultAsync(_ => _.Key == key);
            Monitor.Exit(_locker);
            return setting?.Value;
        }

        public async Task<int?> GetIntSettingAsync(string key)
        {
            Monitor.Enter(_locker);
            var setting = await _context.Setting.FirstOrDefaultAsync(_ => _.Key == key);
            Monitor.Exit(_locker);
            if (setting == null)
            {
                return null;
            }
            return int.Parse(setting.Value);
        }

        public int GetIntSetting(string key)
        {
            Monitor.Enter(_locker);
            var setting = _context.Setting.FirstOrDefault(_ => _.Key == key);
            Monitor.Exit(_locker);
            if (setting == null)
            {
                return default;
            }
            return int.Parse(setting.Value);
        }

        public string GetStringSetting(string key, string defaultValue = null)
        {
            var setting = _context.Setting.FirstOrDefault(_ => _.Key == key);
            if (setting == null)
            {
                return defaultValue;
            }
            return setting?.Value;
        }

        public bool GetBoolSetting(string key, bool defaultBool = false)
        {
            Monitor.Enter(_locker);
            var setting = _context.Setting.FirstOrDefault(_ => _.Key == key);
            Monitor.Exit(_locker);
            if (setting == null)
            {
                return defaultBool;
            }
            return Convert.ToBoolean(int.Parse(setting.Value));
        }

        public void SetSetting(string key, int value)
        {
            var setting = _context.Setting.FirstOrDefault(_ => _.Key == key);
            if (setting != null)
            {
                _context.Setting.Remove(setting);
            }
            _context.Setting.Add(new Setting()
            {
                Key = key,
                Value = value.ToString()
            });
            _context.SaveChanges();
        }

        public void SetSetting(string key, string value)
        {
            Monitor.Enter(_locker);
            var setting = _context.Setting.FirstOrDefault(_ => _.Key == key);
            if (setting != null)
            {
                _context.Setting.Remove(setting);
            }
            _context.Setting.Add(new Setting()
            {
                Key = key,
                Value = value
            });
            _context.SaveChanges();
            Monitor.Exit(_locker);
        }

        public void SetSetting(string key, bool value)
        {
            var setting = _context.Setting.FirstOrDefault(_ => _.Key == key);
            if (setting != null)
            {
                _context.Setting.Remove(setting);
            }
            _context.Setting.Add(new Setting()
            {
                Key = key,
                Value = value ? "1" : "0"
            });
            _context.SaveChanges();
        }
    }
}