// phoneApp Module for selectDevice
angular.module('selectDevices', []).directive('selectDevice', function ($http) {
    return {
        restrict: 'E',
        scope: {
            selectedDevice: '=',
        },
        template: `
      <div class="device-select">
        <select class="form-control" 
                ng-model="selectedDevice" 
                ng-options="device.DeviceId as device.Name for device in devices track by device.DeviceId">
          <option value="">Select a device</option>
        </select>
      </div>
    `,
        link: function (scope) {
            scope.devices = [];
            $http.get('http://localhost:53211/api/devices/all')
                .then(function (response) {
                    scope.devices = response.data;
                })
                .catch(function (error) {
                    console.error('Error fetching devices:', error);
                });
    
        // Watch for changes in selectedDevice
        scope.$watch('selectedDevice', function (newValue, oldValue) {
            if (newValue !== oldValue) {
                console.log('Selected Device:', newValue);
            }
        });
        }
    };
});

