using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using System.Web.Http;
using Van_Rise_Intern_App.Models;

namespace Van_Rise_Intern_App.Controllers
{
    [RoutePrefix("api/clients")]
    public class ClientController : ApiController
    {
        private readonly string _connectionString = "Server=DESKTOP-E4AM5MJ;Database=Van_Rise_Intern;Integrated Security=True;TrustServerCertificate=True;";

        // Adds a new client
        [HttpPost]
        [Route("add")]
        public async Task<IHttpActionResult> AddClient([FromBody] Client client)
        {
            if (client == null || string.IsNullOrWhiteSpace(client.Name))
                return BadRequest("Invalid client details.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.InsertClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Name", client.Name);
                        command.Parameters.AddWithValue("@Type", client.Type);
                        command.Parameters.AddWithValue("@BirthDate", (object)client.BirthDate ?? DBNull.Value);

                        var newClientId = await command.ExecuteScalarAsync();
                        return Ok($"Client added successfully with ID: {newClientId}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return InternalServerError(new Exception("Error adding client. Please try again.", ex));
            }
        }

        // Updates a client
        [HttpPut]
        [Route("update/{id}")]
        public async Task<IHttpActionResult> UpdateClient(int id, [FromBody] Client client)
        {
            if (id <= 0 || client == null || string.IsNullOrWhiteSpace(client.Name))
                return BadRequest("Invalid input data.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.UpdateClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ID", id);
                        command.Parameters.AddWithValue("@Name", client.Name);
                        command.Parameters.AddWithValue("@Type", client.Type);
                        command.Parameters.AddWithValue("@BirthDate", (object)client.BirthDate ?? DBNull.Value);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0
                            ? (IHttpActionResult)Ok("Client updated successfully.")
                            : NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return InternalServerError(new Exception("Error updating client. Please try again.", ex));
            }
        }

        // Retrieves all clients
        [HttpGet]
        [Route("all")]
        public async Task<IHttpActionResult> GetAllClients()
        {
            try
            {
                var clients = new List<Client>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.SelectClient", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                clients.Add(new Client
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    Name = reader["Name"].ToString(),
                                    Type = (ClientType)Convert.ToInt32(reader["Type"]),
                                    BirthDate = reader["BirthDate"] as DateTime?
                                });
                            }
                        }
                    }
                }
                return Ok(clients);
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return InternalServerError(new Exception("Error retrieving clients. Please try again.", ex));
            }
        }

        // GET: api/clients/active-reservations
        [HttpGet]
        [Route("active-reservations")]
        public async Task<IHttpActionResult> GetClientsWithActiveReservations()
        {
            try
            {
                var clients = new List<Client>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.GetClientsWithActiveReservations", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                clients.Add(new Client
                                {
                                    ID = Convert.ToInt32(reader["ClientID"]),
                                    Name = reader["Name"].ToString()
                                });
                            }
                        }
                    }
                }
                return Ok(clients);
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return InternalServerError(new Exception("Error retrieving clients with active reservations. Please try again.", ex));
            }
        }

        // GET: api/clients/phone-number-reservations/{clientId}
        [HttpGet]
        [Route("phone-number-reservations/{clientId}")]
        public async Task<IHttpActionResult> GetPhoneNumberReservationsByClientId(int clientId)
        {
            try
            {
                var reservations = new List<PhoneNumberReservation>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.SearchPhoneNumberReservationsByClientID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ClientID", clientId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                reservations.Add(new PhoneNumberReservation
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    BED = Convert.ToDateTime(reader["BED"]),
                                    EED = reader["EED"] as DateTime?
                                });
                            }
                        }
                    }
                }
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return InternalServerError(new Exception("Error retrieving phone number reservations. Please try again.", ex));
            }
        }

        // GET: api/clients/phone-number/{clientId}
        [HttpGet]
        [Route("phone-number/{clientId}")]
        public async Task<IHttpActionResult> GetPhoneNumberByClientId(int clientId)
        {
            try
            {
                var phoneNumbers = new List<PhoneNumberReservation>();
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.GetPhoneNumberByClientID", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ClientID", clientId);

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
                            {
                                phoneNumbers.Add(new PhoneNumberReservation
                                {
                                    ID = Convert.ToInt32(reader["ID"]),
                                    PhoneNumber = reader["PhoneNumber"].ToString(),
                                    BED = Convert.ToDateTime(reader["BED"]),
                                    EED = reader["EED"] as DateTime?
                                });
                            }
                        }
                    }
                }
                return Ok(phoneNumbers);
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return InternalServerError(new Exception("Error retrieving phone numbers. Please try again.", ex));
            }
        }

        // POST: api/clients/insert-phone-number-reservation
        [HttpPost]
        [Route("insert-phone-number-reservation")]
        public async Task<IHttpActionResult> InsertPhoneNumberReservation([FromBody] PhoneNumberReservation reservation)
        {
            if (reservation == null || string.IsNullOrWhiteSpace(reservation.PhoneNumber))
                return BadRequest("Invalid reservation details.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.InsertPhoneNumberReservation", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@Client_id", reservation.Client_id);
                        command.Parameters.AddWithValue("@PhoneNumber", reservation.PhoneNumber);
                        command.Parameters.AddWithValue("@BED", reservation.BED);
                        command.Parameters.AddWithValue("@EED", (object)reservation.EED ?? DBNull.Value);

                        await command.ExecuteNonQueryAsync();
                        return Ok("Phone number reservation added successfully.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return InternalServerError(new Exception("Error adding phone number reservation. Please try again.", ex));
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
                    using (var command = new SqlCommand("dbo.[SearchReservedPhoneNumbeByClientID]", connection))
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
        public class UnreserveRequest
        {
            public int ReservationID { get; set; }
            public DateTime NewEED { get; set; }
        }
        // PUT: api/clients/unreserve-phone-number
        [HttpPut]
        [Route("unreserve-phone-number")]
        public async Task<IHttpActionResult> UnreservePhoneNumber([FromBody] UnreserveRequest request)
        {
            if (request == null || request.ReservationID <= 0)
                return BadRequest("Invalid request data.");

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.UpdatePhoneNumberReservationEED", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.Parameters.AddWithValue("@ReservationID", request.ReservationID);
                        command.Parameters.AddWithValue("@NewEED", request.NewEED);

                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        return rowsAffected > 0
                            ? (IHttpActionResult)Ok("Phone number unreserved successfully.")
                            : NotFound();
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return InternalServerError(new Exception("Error unreserving phone number. Please try again.", ex));
            }
        }
        // GET: api/clients/counts
        [HttpGet]
        [Route("counts")]
        public async Task<IHttpActionResult> GetClientCounts()
        {
            try
            {
                int individualCount = 0;
                int organizationCount = 0;

                using (var connection = new SqlConnection(_connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("dbo.GetClientCounts", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        using (var reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                individualCount = reader.GetInt32(reader.GetOrdinal("IndividualCount"));
                                organizationCount = reader.GetInt32(reader.GetOrdinal("OrganizationCount"));
                            }
                        }
                    }
                }

                return Ok(new
                {
                    IndividualCount = individualCount,
                    OrganizationCount = organizationCount
                });
            }
            catch (Exception ex)
            {
                // Log the exception (if logging is implemented)
                return InternalServerError(new Exception("Error retrieving client counts. Please try again.", ex));
            }
        }
    }
}