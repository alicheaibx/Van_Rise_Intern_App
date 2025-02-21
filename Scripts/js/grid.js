var app = angular.module('gridApp', ['ClientApp', 'phoneApp']);

app.controller('GridController', function ($scope) {
  $scope.sharedInput = '';
  $scope.modalDeviceName = '';
  $scope.modalTitle = '';
  $scope.modalButtonText = '';
  $scope.modalButtonIcon = '';
  $scope.editingDevice = null;

  $scope.devices = [
    { id: 1, name: 'UltraView 4K TV' },
    { id: 2, name: 'MaxWave Microwave' },
    { id: 3, name: 'SmartCook Oven' },
    { id: 4, name: 'EcoWash Dishwasher' },
    { id: 5, name: 'AirPure Air Purifier' },
    { id: 6, name: 'PowerChill Refrigerator' },
    { id: 7, name: 'SpeedDry Dryer' },
    { id: 8, name: 'CleanWave Vacuum' },
    { id: 9, name: 'BrightLight LED Lamp' },
    { id: 10, name: 'HealthTrack Smartwatch' },
  ];
  

  $scope.filteredDevices = angular.copy($scope.devices);

  $scope.searchDevices = function (searchInput) {
    $scope.filteredDevices = $scope.devices.filter((device) =>
      device.name.toLowerCase().includes((searchInput || '').toLowerCase())
    );
  };

  $scope.openModal = function (action, device) {
    $('#deviceModal').modal('show');
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
      $scope.modalDeviceName = device.name;
      $scope.editingDevice = device;
    }
  };

  $scope.saveDevice = function () {
    if ($scope.editingDevice) {
      $scope.editingDevice.name = $scope.modalDeviceName;
    } else {
      $scope.devices.push({
        id: $scope.devices.length + 1,
        name: $scope.modalDeviceName,
      });
    }
    $scope.filteredDevices = angular.copy($scope.devices);
    $('#deviceModal').modal('hide');
  };

  $scope.deleteDevice = function (id) {
    $scope.devices = $scope.devices.filter((device) => device.id !== id);
    $scope.filteredDevices = angular.copy($scope.devices);
  };
});
