using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using Van_Rise_Intern_App.Models;

namespace Van_Rise_Intern_App.Controllers
{
    [RoutePrefix("api/devices")]
    public class DeviceController : ApiController
    {
        private readonly string _connectionString = "Server=DESKTOP-E4AM5MJ;Database=Van_Rise_Intern;Integrated Security=True;TrustServerCertificate=True;";

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddDevice([FromBody] Device device)
        {
            if (device == null || string.IsNullOrWhiteSpace(device.Name))
                return BadRequest("Device details are invalid.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.AddDevice", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Name", device.Name);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return Ok("Device added successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IHttpActionResult> DeleteDevice(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid device ID.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.DeleteDevice", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Device_Id", id);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return Ok("Device deleted successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAllDevices()
        {
            try
            {
                var devices = new List<Device>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.GetAllDevices", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                devices.Add(new Device
                                {
                                    DeviceId = Convert.ToInt32(reader["Device_Id"]),
                                    Name = reader["Name"].ToString()
                                });
                            }
                        }
                    }
                }
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("get/{id}")]
        public async Task<IHttpActionResult> GetDeviceById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid device ID.");

            try
            {
                Device device = null;
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.GetDeviceById", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Device_Id", id);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                device = new Device
                                {
                                    DeviceId = Convert.ToInt32(reader["Device_Id"]),
                                    Name = reader["Name"].ToString()
                                };
                            }
                        }
                    }
                }
                return device != null ? (IHttpActionResult)Ok(device) : NotFound();
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IHttpActionResult> UpdateDevice(int id, [FromBody] Device device)
        {
            if (id <= 0 || device == null || string.IsNullOrWhiteSpace(device.Name))
                return BadRequest("Invalid input data.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.UpdateDevice", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Device_Id", id);
                        command.Parameters.AddWithValue("@Name", device.Name);
                        await command.ExecuteNonQueryAsync();
                    }
                }
                return Ok("Device updated successfully.");
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("search")]
        public async Task<IHttpActionResult> SearchDevices(string searchTerm)
        {
            var devices = new List<Device>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("[dbo].[SearchDevicesByName]", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SearchTerm", searchTerm ?? string.Empty);
                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                devices.Add(new Device
                                {
                                    DeviceId = reader.GetInt32(0),
                                    Name = reader.GetString(1)
                                });
                            }
                        }
                    }
                }
                return Ok(devices);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }
}
