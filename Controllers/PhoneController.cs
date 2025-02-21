using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using Van_Rise_Intern_App.Models;

namespace Van_Rise_Intern_App.Controllers
{
    [RoutePrefix("api/phones")]
    public class PhoneController : ApiController
    {
        private readonly string _connectionString = "Server=DESKTOP-E4AM5MJ;Database=Van_Rise_Intern;Integrated Security=True;TrustServerCertificate=True;";

        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddPhone([FromBody] Phone phone)
        {
            if (phone == null || string.IsNullOrWhiteSpace(phone.Number))
                return BadRequest("Phone details are invalid.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.InsertPhone", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Number", phone.Number);
                        command.Parameters.AddWithValue("@Device_id", phone.DeviceId);
                        var newPhoneId = await command.ExecuteScalarAsync();
                        return Ok($"Phone added successfully with ID: {newPhoneId}");
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpDelete]
        [Route("delete/{id}")]
        public async Task<IHttpActionResult> DeletePhone(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid phone ID.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.DeletePhone", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", id);
                        var rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok($"Phone with ID {id} deleted successfully.");
                        else
                            return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAllPhones()
        {
            try
            {
                var phones = new List<Phone>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("GetDevicesWithPhones", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                phones.Add(new Phone
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Number = reader.GetString(reader.GetOrdinal("Number")),
                                    DeviceId = reader.GetInt32(reader.GetOrdinal("DeviceId")),
                                     DeviceName = reader.GetString(reader.GetOrdinal("DeviceName")),
                                });
                            }
                        }
                    }
                }
                return Ok(phones);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPut]
        [Route("update/{id}")]
        public async Task<IHttpActionResult> UpdatePhone(int id, [FromBody] Phone phone)
        {
            if (id <= 0 || phone == null || string.IsNullOrWhiteSpace(phone.Number))
                return BadRequest("Invalid input data.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.UpdatePhone", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", id);
                        command.Parameters.AddWithValue("@Number", phone.Number);
                        command.Parameters.AddWithValue("@Device_id", phone.DeviceId);
                        var rowsAffected = await command.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                            return Ok($"Phone with ID {id} updated successfully.");
                        else
                            return NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("search")]
        public async Task<IHttpActionResult> SearchPhones(string searchTerm)
        {
            var phones = new List<Phone>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.SearchPhones", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@SearchTerm", searchTerm ?? string.Empty);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                phones.Add(new Phone
                                {
                                    Id = Convert.ToInt32(reader["ID"]),
                                    Number = reader["Number"].ToString(),
                                    DeviceId = Convert.ToInt32(reader["Device_id"])
                                });
                            }
                        }
                    }
                }
                return Ok(phones);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("by-device/{deviceId}")]
        public async Task<IHttpActionResult> SelectPhoneByDeviceID(int deviceId)
        {
            if (deviceId <= 0)
                return BadRequest("Invalid device ID.");

            try
            {
                var phones = new List<Phone>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("SelectPhoneByDeviceID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@DeviceID", deviceId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                phones.Add(new Phone
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    Number = reader.GetString(reader.GetOrdinal("Number")),
                                    DeviceId = reader.GetInt32(reader.GetOrdinal("Device_id"))
                                });
                            }
                        }
                    }
                }
                return Ok(phones);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpGet]
        [Route("select")]
        public async Task<IHttpActionResult> SelectAllPhones()
        {
            try
            {
                var phones = new List<Phone>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.SelectAllPhone", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        // No parameter is passed, as we are selecting all records
                        command.Parameters.Clear();

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                phones.Add(new Phone
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                    Number = reader.GetString(reader.GetOrdinal("Number")),
                                    DeviceId = reader.GetInt32(reader.GetOrdinal("Device_id"))
                                });
                            }
                        }
                    }
                }

                if (phones.Count == 0)
                {
                    return NotFound(); // Return 404 if no phone records found
                }

                return Ok(phones);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // Handle any exceptions
            }
        }
        [HttpGet]
        [Route("available")]
        public async Task<IHttpActionResult> GetAvailablePhoneNumbers()
        {
            try
            {
                var availablePhones = new List<Phone>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.GetAvailablePhoneNumbers", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                availablePhones.Add(new Phone
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("ID")),
                                    Number = reader.GetString(reader.GetOrdinal("Number"))
                                });
                            }
                        }
                    }
                }

                if (availablePhones.Count == 0)
                {
                    return NotFound(); // Return 404 if no available phone numbers are found
                }

                return Ok(availablePhones);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex); // Handle any exceptions
            }
        }
    }
}
