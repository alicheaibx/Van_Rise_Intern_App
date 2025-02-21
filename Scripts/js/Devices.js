var app = angular.module('gridApp', []);

app.controller('GridController', function ($scope, $http) { // Inject $http
    // Initialize variables
    $scope.sharedInput = '';
    $scope.modalDeviceName = '';
    $scope.modalTitle = '';
    $scope.modalButtonText = '';
    $scope.modalButtonIcon = '';
    $scope.editingDevice = null;

    $scope.devices = [];
    $scope.filteredDevices = [];

    // Load all devices from the backend
    $scope.loadDevices = function () {
        $http.get('http://localhost:53211/api/devices/all')
            .then(function (response) {
                $scope.devices = response.data;
                $scope.filteredDevices = angular.copy($scope.devices);
            })
            .catch(function (error) {
                console.error('Error loading devices:', error);
                alert('Failed to load devices. Please try again.');
            });
    };

    // Search Functionality
    $scope.searchDevices = function (searchInput) {
        $http.get('http://localhost:53211/api/devices/search', {
            params: { searchTerm: searchInput || '' }
        })
            .then(function (response) {
                $scope.filteredDevices = response.data;
            })
            .catch(function (error) {
                console.error('Error searching devices:', error);
                alert('Failed to search devices. Please try again.');
            });
    };

    // Open Modal for Add/Edit
    $scope.openModal = function (action, device) {
        $('#deviceModal').modal('show'); // Consider using AngularJS directives for modals
        if (action === 'add') {
            $scope.modalTitle = 'Add Device';
            $scope.modalButtonText = 'Save';
            $scope.modalButtonIcon = 'fa fa-plus';
            $scope.modalDeviceName = '';
            $scope.editingDevice = null;
        } else if (action === 'edit') {
            $scope.modalTitle = 'Edit Device';
            $scope.modalButtonText = 'Update';
            $scope.modalButtonIcon = 'fa fa-check';
            $scope.modalDeviceName = device.Name;
            $scope.editingDevice = device;
        }
    };

    // Close Modal
    $scope.closeModal = function () {
        $('#deviceModal').modal('hide'); // Consider using AngularJS directives for modals
        $scope.modalDeviceName = '';
        $scope.editingDevice = null;
    };

    // Save Device (Add/Edit)
    $scope.saveDevice = function () {
        if ($scope.modalDeviceName.trim() === '') {
            alert('Device name cannot be empty.');
            return;
        }

        if ($scope.editingDevice) {
            // Update device
            $scope.updateDevice($scope.editingDevice.DeviceId, $scope.modalDeviceName);
        } else {
            // Add new device
            $scope.addDevice($scope.modalDeviceName);
        }
    };

    // Add a new device
    $scope.addDevice = function (deviceName) {
        $http.post('http://localhost:53211/api/devices/add', { name: deviceName })
            .then(function () {
                $scope.loadDevices();
                $scope.closeModal();
            })
            .catch(function (error) {
                console.error('Error adding device:', error);
                alert('Failed to add device. Please try again.');
            });
    };

    // Update an existing device
    $scope.updateDevice = function (id, deviceName) {
        $http.put(`http://localhost:53211/api/devices/update/${id}`, { name: deviceName })
            .then(function (response) {
                $scope.loadDevices();
                $scope.closeModal();
            })
            .catch(function (error) {
                console.error('Error updating device:', error);
                alert('Failed to update device. Please try again.');
            });
    };

    // Delete Device
    $scope.deleteDevice = function (id) {
        if (confirm('Are you sure you want to delete this device?')) {
            $http.delete(`http://localhost:53211/api/devices/delete/${id}`)
                .then(function () {
                    $scope.devices = $scope.devices.filter((device) => device.DeviceId !== id);
                    $scope.filteredDevices = angular.copy($scope.devices);
                })
                .catch(function (error) {
                    console.error('Error deleting device:', error);
                    alert('Failed to delete device. Please try again.');
                });
        }
    };

    // Load devices on initialization
    $scope.loadDevices();
});