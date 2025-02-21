angular.module('phoneReservationApp', ['selectClients', 'selectPhones'])
    .directive('reservationModal', function ($http) {
        return {
            restrict: 'E',
            scope: {
                onSave: '&',
                onClose: '&',
                selectedClient: '=' // Binding for the selected client
            },
            template: `
                <div class="modal fade" id="reservationModal" tabindex="-1" role="dialog" aria-labelledby="reservationModalLabel" aria-hidden="true">
                    <div class="modal-dialog" role="document">
                        <div class="modal-content">
                            <div class="modal-header">
                                <h5 class="modal-title" id="reservationModalLabel">Add Reservation</h5>
                                <button type="button" class="close" ng-click="close()" aria-label="Close">
                                    <span aria-hidden="true">&times;</span>
                                </button>
                            </div>
                            <div class="modal-body">
                                <div class="form-group">
                                    <label for="clientName">Client Name</label>
                                    <input type="text" class="form-control" id="clientName" ng-model="selectedClient.Name" disabled>
                                </div>
                                <div class="form-group">
                                    <label for="phoneNumber">Phone Number</label>
                                    <select-phone selected-phone="newReservation.phone" api-url="http://localhost:53211/api/phones/available"></select-phone>
                                </div>
                                <div class="form-group">
                                    <label for="beginEffectiveDate">Begin Effective Date</label>
                                    <input type="date" class="form-control" id="beginEffectiveDate" ng-model="newReservation.beginEffectiveDate" disabled>
                                </div>
                                <div class="form-group">
                                    <label for="endEffectiveDate">End Effective Date</label>
                                    <input type="date" class="form-control" id="endEffectiveDate" ng-model="newReservation.endEffectiveDate">
                                </div>
                            </div>
                            <div class="modal-footer">
                                <button type="button" class="btn btn-secondary" ng-click="close()">Close</button>
                                <button type="button" class="btn btn-primary" ng-click="save()">Save</button>
                            </div>
                        </div>
                    </div>
                </div>
            `,
            link: function (scope) {
                // Initialize the reservation object
                scope.newReservation = {
                    clientId: null, // Initially set to null
                    phone: null,
                    beginEffectiveDate: new Date().toISOString().split('T')[0], // Set to today's date
                    endEffectiveDate: null
                };

                // Watch for changes in selectedClient and update clientId in newReservation
                scope.$watch('selectedClient', function (newClient) {
                    if (newClient) {
                        scope.newReservation.clientId = newClient.ID; // Update clientId
                        console.log('Updated clientId in newReservation:', scope.newReservation.clientId);
                    }
                });

                // Watch for changes in the newReservation object
                scope.$watch('newReservation', function (newValue) {
                    console.log('Current reservation data:', newValue);
                }, true); // The third parameter 'true' enables deep watching

                scope.save = function () {
                    // Validate required fields (excluding endEffectiveDate)
                    if (!scope.newReservation.clientId || !scope.newReservation.phone) {
                        alert('Please fill out all required fields (Client and Phone Number).');
                        return;
                    }

                    // Validate beginEffectiveDate
                    const beginDate = new Date(scope.newReservation.beginEffectiveDate);
                    if (isNaN(beginDate.getTime())) {
                        alert('Invalid Begin Effective Date format. Please enter a valid date.');
                        return;
                    }

                    // Validate endEffectiveDate (if provided)
                    const endDate = scope.newReservation.endEffectiveDate ? new Date(scope.newReservation.endEffectiveDate) : null;
                    if (endDate && isNaN(endDate.getTime())) {
                        alert('Invalid End Effective Date format. Please enter a valid date.');
                        return;
                    }

                    // Ensure endEffectiveDate is after beginEffectiveDate (if provided)
                    if (endDate && endDate < beginDate) {
                        alert('End Effective Date must be after Begin Effective Date.');
                        return;
                    }

                    // Prepare the data to be sent to the API
                    const reservationData = {
                        Client_id: scope.newReservation.clientId, // Use the stored client ID
                        PhoneNumber: scope.newReservation.phone.Number,
                        BED: scope.newReservation.beginEffectiveDate, // Already formatted as "YYYY-MM-DD"
                        EED: scope.newReservation.endEffectiveDate || null // Send null if endEffectiveDate is not provided
                    };

                    console.log('Sending reservation data:', reservationData); // Debugging

                    // Send a POST request to the API
                    $http.post('http://localhost:53211/api/phone-reservations/insert', reservationData)
                        .then(function (response) {
                            // Handle success
                            console.log('Reservation saved successfully:', response.data);
                            scope.onSave({ reservation: scope.newReservation });
                            scope.close();
                        })
                        .catch(function (error) {
                            // Handle error
                            console.error('Error saving reservation:', error);
                            alert('An error occurred while saving the reservation. Please try again.');
                        });
                };

                scope.close = function () {
                    scope.onClose();
                    scope.newReservation = {
                        clientId: null, // Reset to null
                        phone: null,
                        beginEffectiveDate: new Date().toISOString().split('T')[0], // Reset to today's date
                        endEffectiveDate: null
                    }; // Reset the form
                };
            }
        };
    });