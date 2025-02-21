angular.module('clientApp', ['selectClientType', 'selectPhones'])
    .controller('ClientController', ['$scope', '$http', function ($scope, $http) {
        // Initialize variables
        $scope.clients = [];
        $scope.filteredClients = [];
        $scope.activeReservations = [];
        $scope.searchName = '';
        $scope.filterType = '';
        $scope.modalTitle = '';
        $scope.currentClient = {};
        $scope.modalButtonText = '';
        $scope.modalButtonIcon = '';
        $scope.clientReservations = [];
        $scope.selectedClientId = null;
        $scope.selectedClient = null; // For reservation modal

        // Initialize reservation object
        $scope.newReservation = {
            clientId: null,
            phone: null,
            beginEffectiveDate: new Date().toISOString().split('T')[0], // Today's date
            endEffectiveDate: null
        };

        // Fetch all clients
        $scope.getClients = function () {
            $http.get('http://localhost:53211/api/clients/all')
                .then(function (response) {
                    console.log('Clients fetched:', response.data);
                    $scope.clients = response.data;
                    $scope.filteredClients = response.data;
                    $scope.getActiveReservations();
                })
                .catch(function (error) {
                    console.error('Error fetching clients:', error);
                    alert('Error fetching clients.');
                });
        };

        // Fetch active reservations
        $scope.getActiveReservations = function () {
            $http.get('http://localhost:53211/api/clients/active-reservations')
                .then(function (response) {
                    console.log('Active reservations fetched:', response.data);
                    $scope.activeReservations = response.data;
                })
                .catch(function (error) {
                    console.error('Error fetching active reservations:', error);
                    alert('Error fetching active reservations.');
                });
        };

        // Check if a client has an active reservation
        $scope.hasActiveReservation = function (clientId) {
            return $scope.activeReservations.some(reservation => reservation.ID === clientId);
        };

        // Open Reservation Modal
        $scope.openReservationModal = function (client) {
            if (!client) {
                console.error('Client is missing.');
                return;
            }
            $scope.selectedClient = client; // Set the selected client
            $scope.newReservation.clientId = client.ID; // Set the client ID for the reservation
            $('#reservationModal').modal('show'); // Open the modal
        };

        // Save Reservation
        $scope.saveReservation = function () {
            // Validation
            if (!$scope.newReservation.clientId || !$scope.newReservation.phone) {
                alert('Please select a client and phone number.');
                return;
            }

            const beginDate = new Date($scope.newReservation.beginEffectiveDate);
            const endDate = $scope.newReservation.endEffectiveDate ?
                new Date($scope.newReservation.endEffectiveDate) : null;

            if (endDate && endDate < beginDate) {
                alert('End date must be after begin date.');
                return;
            }

            // Prepare payload
            const payload = {
                Client_id: $scope.newReservation.clientId,
                PhoneNumber: $scope.newReservation.phone.Number,
                BED: $scope.newReservation.beginEffectiveDate,
                EED: endDate ? endDate.toISOString().split('T')[0] : null
            };

            // Save to API
            $http.post('http://localhost:53211/api/clients/insert-phone-number-reservation', payload)
                .then(function (response) {
                    alert('Reservation saved successfully!');
                    $('#reservationModal').modal('hide');
                    $scope.getClients(); // Refresh list
                })
                .catch(function (error) {
                    console.error('Reservation error:', error);
                    alert('Error saving reservation: ' + error.data);
                });
        };

        // Close Reservation Modal
        $scope.closeReservationModal = function () {
            $('#reservationModal').modal('hide');
            $scope.selectedClient = null; // Reset selected client
            $scope.newReservation = {
                clientId: null,
                phone: null,
                beginEffectiveDate: new Date().toISOString().split('T')[0],
                endEffectiveDate: null
            };
        };

        // Open Modal for Add/Edit Client
        $scope.openModal = function (action, client) {
            $('#clientModal').modal('show');
            if (action === 'add') {
                $scope.modalTitle = 'Add Client';
                $scope.modalButtonText = 'Save';
                $scope.modalButtonIcon = 'fa fa-plus';
                $scope.currentClient = { Name: '', Type: '', BirthDate: null };
            } else if (action === 'edit') {
                $scope.modalTitle = 'Edit Client';
                $scope.modalButtonText = 'Update';
                $scope.modalButtonIcon = 'fa fa-check';
                $scope.currentClient = angular.copy(client);
            }
        };

        // Apply Filters to Client List
        $scope.applyFilters = function () {
            const nameFilter = $scope.searchName.trim();
            const typeFilter = $scope.filterType;

            let filteredClients = $scope.clients;
            if (nameFilter) {
                filteredClients = filteredClients.filter(client => client.Name.toLowerCase().includes(nameFilter.toLowerCase()));
            }

            if (typeFilter) {
                filteredClients = filteredClients.filter(client => client.Type == typeFilter);
            }

            $scope.filteredClients = filteredClients;
        };

        // Save or Update a Client
        $scope.saveClient = function () {
            if (!$scope.currentClient.Name || !$scope.currentClient.Type) {
                alert('Please fill out all required fields (Name and Type).');
                return;
            }

            const clientData = {
                Name: $scope.currentClient.Name,
                Type: $scope.currentClient.Type,
                BirthDate: $scope.currentClient.BirthDate
            };

            if ($scope.modalButtonText === 'Update') {
                const clientId = $scope.currentClient.ID;
                $http.put(`http://localhost:53211/api/clients/update/${clientId}`, clientData)
                    .then(function () {
                        alert('Client updated successfully.');
                        $scope.getClients();
                    })
                    .catch(function (error) {
                        console.error('Error updating client:', error);
                        alert('Error updating client.');
                    });
            } else {
                $http.post('http://localhost:53211/api/clients/add', clientData)
                    .then(function () {
                        alert('Client added successfully.');
                        $scope.getClients();
                    })
                    .catch(function (error) {
                        console.error('Error adding client:', error);
                        alert('Error adding client.');
                    });
            }

            $scope.closeModal();
            $scope.currentClient = {};
        };

        // Delete a Client
        $scope.deleteClient = function (id) {
            if (confirm('Are you sure you want to delete this client?')) {
                $http.delete(`http://localhost:53211/api/clients/delete/${id}`)
                    .then(function (response) {
                        alert(response.data);
                        $scope.getClients();
                    })
                    .catch(function (error) {
                        console.error('Error deleting client:', error);
                        alert('Error deleting client.');
                    });
            }
        };


        // Close Client Modal
        $scope.closeModal = function () {
            $('#clientModal').modal('hide');
        };

        // Open Manage Reservation Modal
        $scope.openManageReservationModal = function (clientId) {
            $scope.selectedClientId = clientId;
            $scope.getClientReservations(clientId);
            $('#manageReservationModal').modal('show');
        };

        // Fetch reservations for the selected client
        $scope.getClientReservations = function (clientId) {
            // Construct the URL with the clientId parameter
            const url = `http://localhost:53211/api/clients/search-by-client?clientID=${clientId}`;

            $http.get(url)
                .then(function (response) {
                    console.log('Client reservations fetched:', response.data);
                    $scope.clientReservations = response.data;
                })
                .catch(function (error) {
                    console.error('Error fetching reservations:', error);
                    alert('Error fetching reservations: ' + (error.data || error.statusText));
                });
        };

        // Unreserve Phone Number
        $scope.unreservePhoneNumber = function (reservation) {
            if (!reservation || !reservation.ID) {
                alert('No reservation selected.');
                return;
            }

            const newEED = new Date(); // Set the new EED to today's date or any other logic you want
            const payload = {
                ReservationID: reservation.ID,
                NewEED: newEED.toISOString().split('T')[0] // Format to YYYY-MM-DD
            };

            $http.put('http://localhost:53211/api/clients/unreserve-phone-number', payload)
                .then(function (response) {
                    alert('Phone number unreserved successfully!');
                    $scope.getClientReservations($scope.selectedClientId); // Refresh the reservations
                })
                .catch(function (error) {
                    console.error('Error unreserving phone number:', error);
                    alert('Error unreserving phone number: ' + (error.data || error.statusText));
                });
        };

        // Close Manage Reservation Modal
        $scope.closeManageReservationModal = function () {
            $('#manageReservationModal').modal('hide');
            $scope.clientReservations = [];
            $scope.selectedClientId = null;
        };

        // Initialize the Controller
        $scope.init = function () {
            console.log("Initializing the controller...");
            $scope.getClients();
        };
        $scope.init();
    }]);