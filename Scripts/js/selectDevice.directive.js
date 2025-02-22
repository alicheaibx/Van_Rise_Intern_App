angular.module('selectDevices', []).directive('selectDevice', function ($http) {
    return {
        restrict: 'E',
        scope: {
            selectedDevice: '=', // Two-way binding for the selected value
            apiUrl: '@',       // API endpoint URL (passed as a string)
            placeholder: '@',   // Placeholder text for the dropdown
            labelField: '@',    // Field to use as the label in the dropdown
            valueField: '@',    // Field to use as the value in the dropdown
            onSelect: '&'       // Callback function when a device is selected
        },
        template: `
      <div class="device-select">
        <select class="form-control" 
                ng-model="selectedDevice" 
                ng-options="device[valueField] as device[labelField] for device in devices track by device[valueField]">
          <option value="">{{ placeholder }}</option>
        </select>
      </div>
    `,
        link: function (scope) {
            // Initialize devices array
            scope.devices = [];

            // Fetch devices from the API
            if (scope.apiUrl) {
                $http.get(scope.apiUrl)
                    .then(function (response) {
                        scope.devices = response.data;
                    })
                    .catch(function (error) {
                        console.error('Error fetching devices:', error);
                    });
            }

            // Watch for changes in selectedDevice
            scope.$watch('selectedDevice', function (newValue, oldValue) {
                if (newValue !== oldValue) {
                    console.log('Selected Device:', newValue);
                    // Trigger the onSelect callback if provided
                    if (scope.onSelect) {
                        scope.onSelect({ selectedValue: newValue });
                    }
                }
            });
        }
    };
});