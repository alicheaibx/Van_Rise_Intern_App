using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using Van_Rise_Intern_App.Models;

namespace Van_Rise_Intern_App.Controllers
{
    [RoutePrefix("api/phone-reservations")]
    public class PhoneNumberReservationController : ApiController
    {
        private readonly string _connectionString = "Server=DESKTOP-E4AM5MJ;Database=Van_Rise_Intern;Integrated Security=True;TrustServerCertificate=True;";

        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAllPhoneNumberReservations()
        {
            try
            {
                var reservations = new List<PhoneNumberReservation>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.GetAllPhoneNumberReservations", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reservations.Add(new PhoneNumberReservation
                                {
                                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                    Client_id = reader.GetInt32(reader.GetOrdinal("Client_id")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    BED = reader.GetDateTime(reader.GetOrdinal("BED")),
                                    EED = reader.IsDBNull(reader.GetOrdinal("EED")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EED"))
                                });
                            }
                        }
                    }
                }
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("with-client")]
        public async Task<IHttpActionResult> GetPhoneNumberReservationsWithClient()
        {
            try
            {
                var reservations = new List<PhoneNumberReservation>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("dbo.GetPhoneNumberReservationsWithClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reservations.Add(new PhoneNumberReservation
                                {
                                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                    Client_id = reader.GetInt32(reader.GetOrdinal("Client_id")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    BED = reader.GetDateTime(reader.GetOrdinal("BED")),
                                    EED = reader.IsDBNull(reader.GetOrdinal("EED")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EED")),
                                    ClientName = reader.GetString(reader.GetOrdinal("ClientName")) // Assuming Client table has a Name column
                                });
                            }
                        }
                    }
                }
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("search-by-phone")]
        public async Task<IHttpActionResult> SearchByPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return BadRequest("Phone number is required.");

            var reservations = new List<PhoneNumberReservation>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.SearchPhoneNumberReservationsByPhone", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@PhoneNumber", phoneNumber);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reservations.Add(new PhoneNumberReservation
                                {
                                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                    Client_id = reader.GetInt32(reader.GetOrdinal("Client_id")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    BED = reader.GetDateTime(reader.GetOrdinal("BED")),
                                    EED = reader.IsDBNull(reader.GetOrdinal("EED")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EED"))
                                });
                            }
                        }
                    }
                }
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("search-by-client")]
        public async Task<IHttpActionResult> SearchByClientID(int clientID)
        {
            if (clientID <= 0)
                return BadRequest("Invalid client ID.");

            var reservations = new List<PhoneNumberReservation>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.SearchPhoneNumberReservationsByClientID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ClientID", clientID);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reservations.Add(new PhoneNumberReservation
                                {
                                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                    Client_id = reader.GetInt32(reader.GetOrdinal("Client_id")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    BED = reader.GetDateTime(reader.GetOrdinal("BED")),
                                    EED = reader.IsDBNull(reader.GetOrdinal("EED")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EED"))
                                });
                            }
                        }
                    }
                }
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpPost]
        [Route("insert")]
        public async Task<IHttpActionResult> InsertPhoneNumberReservation([FromBody] PhoneNumberReservation reservation)
        {
            if (reservation == null)
            {
                return BadRequest("Invalid reservation data.");
            }

            try
            {
                // Log the received data for debugging
                Console.WriteLine($"Received reservation: ClientId={reservation.Client_id}, PhoneNumber={reservation.PhoneNumber}, BED={reservation.BED}, EED={reservation.EED}");

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.InsertPhoneNumberReservation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Add parameters
                        command.Parameters.AddWithValue("@Client_id", reservation.Client_id);
                        command.Parameters.AddWithValue("@PhoneNumber", reservation.PhoneNumber);
                        command.Parameters.AddWithValue("@BED", reservation.BED);
                        command.Parameters.AddWithValue("@EED", (object)reservation.EED ?? DBNull.Value);

                        // Execute the stored procedure
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return Ok("Phone number reservation inserted successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception for debugging
                Console.WriteLine($"Error inserting reservation: {ex.Message}");
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("by-client/{clientID}")]
        public async Task<IHttpActionResult> GetPhoneNumberReservationsByClientID(int clientID)
        {
            if (clientID <= 0)
                return BadRequest("Invalid client ID.");

            var reservations = new List<PhoneNumberReservation>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.GetPhoneNumberByClientID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ClientID", clientID);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reservations.Add(new PhoneNumberReservation
                                {
                                    ID = reader.GetInt32(reader.GetOrdinal("ID")),
                                    PhoneNumber = reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                    BED = reader.GetDateTime(reader.GetOrdinal("BED")),
                                    EED = reader.IsDBNull(reader.GetOrdinal("EED")) ? (DateTime?)null : reader.GetDateTime(reader.GetOrdinal("EED"))
                                });
                            }
                        }
                    }
                }
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

        [HttpGet]
        [Route("active-inactive-reservations")]
        public async Task<IHttpActionResult> GetActiveInactiveReservationsByDevice()
        {
            try
            {
                var results = new List<DeviceReservationStatus>();

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();

                    using (var command = new SqlCommand("dbo.CountActiveInactiveReservationsByDevice", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                results.Add(new DeviceReservationStatus
                                {
                                    DeviceID = reader.GetInt32(reader.GetOrdinal("DeviceID")),
                                    DeviceName = reader.GetString(reader.GetOrdinal("DeviceName")),
                                    ActiveReservations = reader.GetInt32(reader.GetOrdinal("ActiveReservations")),
                                    InactiveReservations = reader.GetInt32(reader.GetOrdinal("InactiveReservations")),
                                    PhoneNumber= reader.GetString(reader.GetOrdinal("PhoneNumber")),
                                });
                            }
                        }
                    }
                }

                return Ok(results);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
    }

    // Define a model to represent the result of the stored procedure
    public class DeviceReservationStatus
    {
        public int DeviceID { get; set; }
        public String DeviceName { get; set; }
        public int ActiveReservations { get; set; }
        public int InactiveReservations { get; set; }
        public string PhoneNumber { get; set; }
    }

  
}