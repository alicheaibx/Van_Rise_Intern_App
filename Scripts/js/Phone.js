angular.module('phoneApp', ['selectDevices']) // Corrected module name
    .controller('PhoneController', ['$scope', '$http', function ($scope, $http) {
        $scope.phones = [];
        $scope.filteredPhones = [];
        $scope.searchNumber = '';
        $scope.filterDevice = ''; // For filtering in applyFilters
        $scope.modalTitle = '';
        $scope.modalButtonText = '';
        $scope.modalButtonIcon = '';
        $scope.modalPhoneNumber = '';
        $scope.modalDeviceId = ''; // This now binds to selectDevice directive
        $scope.modalPhoneId = null;

        // Fetch All Phones
        $scope.getAllPhones = function () {
            $http.get('http://localhost:53211/api/phones/all')
                .then(function (response) {
                    $scope.phones = response.data;
                    $scope.filteredPhones = response.data;
                })
                .catch(function (error) {
                    console.error('Error fetching phones:', error);
                    alert('Error fetching phones.');
                });
        };

        // Apply Filters
        $scope.applyFilters = function () {
            const phoneFilter = $scope.searchNumber.trim();
            const deviceFilter = $scope.filterDevice;

            let filteredPhones = $scope.phones;

            if (phoneFilter) {
                filteredPhones = filteredPhones.filter(phone => phone.Number.includes(phoneFilter));
            }

            if (deviceFilter) {
                filteredPhones = filteredPhones.filter(phone => phone.DeviceId == deviceFilter);
            }

            $scope.filteredPhones = filteredPhones;
        };

        // Open Modal for Add/Edit
        $scope.openModal = function (action, phone) {
            if (action === 'edit') {
                $scope.modalTitle = 'Edit Phone';
                $scope.modalPhoneNumber = phone.Number;
                $scope.modalDeviceId = phone.DeviceId;
                $scope.modalPhoneId = phone.Id;
                $scope.modalButtonText = 'Update Phone';
                $scope.modalButtonIcon = 'fa fa-edit';
            } else {
                $scope.modalTitle = 'Add Phone';
                $scope.modalPhoneNumber = '';
                $scope.modalDeviceId = '';
                $scope.modalPhoneId = null;
                $scope.modalButtonText = 'Add Phone';
                $scope.modalButtonIcon = 'fa fa-plus';
            }
            $('#phoneModal').modal('show');
        };

        // Save Phone (Add or Update)
        $scope.savePhone = function () {
            if (!$scope.modalPhoneNumber || !$scope.modalDeviceId) {
                alert('Please fill in all required fields (Phone Number and Device).');
                return;
            }

            const phoneData = {
                Number: $scope.modalPhoneNumber,
                DeviceId: $scope.modalDeviceId
            };

            if ($scope.modalButtonText === 'Update Phone') {
                const phoneId = $scope.modalPhoneId;
                console.log('Updating phone with ID:', phoneId, 'Data:', phoneData);
                $http.put(`http://localhost:53211/api/phones/update/${phoneId}`, phoneData)
                    .then(function () {
                        alert('Phone updated successfully.');
                        $scope.getAllPhones();
                    })
                    .catch(function (error) {
                        console.error('Error updating phone:', error);
                        alert('Error updating phone.');
                    });
            } else {
                console.log('Adding new phone:', phoneData);
                $http.post('http://localhost:53211/api/phones/add', phoneData)
                    .then(function () {
                        alert('Phone added successfully.');
                        $scope.getAllPhones();
                    })
                    .catch(function (error) {
                        console.error('Error adding phone:', error);
                        alert('Error adding phone.');
                    });
            }

            $scope.closeModal();
        };

        // Delete Phone
        $scope.deletePhone = function (id) {
            if (confirm('Are you sure you want to delete this phone?')) {
                $http.delete(`http://localhost:53211/api/phones/delete/${id}`)
                    .then(function () {
                        alert('Phone deleted successfully.');
                        $scope.getAllPhones();
                    })
                    .catch(function (error) {
                        console.error('Error deleting phone:', error);
                        alert('Error deleting phone.');
                    });
            }
        };

        // Close Modal
        $scope.closeModal = function () {
            $('#phoneModal').modal('hide');
        };

        // Initialize
        $scope.init = function () {
            $scope.getAllPhones();
            console.log("Application Initialized");
        };

        $scope.init();
    }]);
