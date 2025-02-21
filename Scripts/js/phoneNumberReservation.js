angular.module('phoneReservationApp', ['selectClients','selectPhones'])
    .controller('PhoneReservationController', ['$scope', '$http', function ($scope, $http) {
        $scope.reservations = [];
        $scope.filteredReservations = [];
        $scope.selectedPhone = '';
        $scope.selectedClient = '';

        // Fetch All Reservations
        $scope.getAllReservations = function () {
            $http.get('http://localhost:53211/api/phone-reservations/with-client')
                .then(function (response) {
                    $scope.reservations = response.data;
                    $scope.filteredReservations = response.data;
                })
                .catch(function (error) {
                    console.error('Error fetching reservations:', error);
                });
        };

        $scope.applyFilters = function () {
            const phoneFilter = $scope.selectedPhone ? String($scope.selectedPhone.Number).trim() : '';
            const clientFilter = $scope.selectedClient ? String($scope.selectedClient.Name).trim() : ''; // Filter by client name

            let filteredReservations = $scope.reservations;

            if (phoneFilter) {
                filteredReservations = filteredReservations.filter(reservation =>
                    String(reservation.PhoneNumber).trim() === phoneFilter
                );
            }

            if (clientFilter) {
                filteredReservations = filteredReservations.filter(reservation =>
                    String(reservation.ClientName).trim() === clientFilter
                );
            }

            $scope.filteredReservations = filteredReservations;
        };


        // Delete Reservation
        $scope.deleteReservation = function (id) {
            if (confirm('Are you sure you want to delete this reservation?')) {
                $http.delete(`http://localhost:53211/api/phone-reservations/delete/${id}`)
                    .then(function () {
                        alert('Reservation deleted successfully.');
                        $scope.getAllReservations();
                    })
                    .catch(function (error) {
                        console.error('Error deleting reservation:', error);
                    });
            }
        };

        // Initialize
        $scope.init = function () {
            $scope.getAllReservations();
        };

        $scope.init();
    }]);