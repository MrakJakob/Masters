import numpy as np
import matplotlib.pyplot as plt

from ex2_utils import create_epanechnik_kernel, generate_responses_1


import numpy as np


def mean_shift(
    data, kernel, bandwidth=None, max_iter=10000, tol=0.0025, start_position=(0, 0)
):
    # Define the range of coordinates
    x_range = np.arange(kernel.shape[0]) - kernel.shape[0] // 2
    y_range = np.arange(kernel.shape[1]) - kernel.shape[1] // 2

    # Create coordinate matrices for X and Y directions using meshgrid
    xi, yi = np.meshgrid(x_range, y_range)

    path = np.array(start_position)

    x, y = start_position
    for i in range(max_iter):
        # get data points within the kernel window
        img_x_range = round(x) - kernel.shape[0] // 2, round(x) + kernel.shape[0] // 2
        img_y_range = round(y) - kernel.shape[1] // 2, round(y) + kernel.shape[1] // 2
        window = data[
            img_x_range[0] : img_x_range[1] + 1, img_y_range[0] : img_y_range[1] + 1
        ]

        if (window.shape[0] != kernel.shape[0]) or (window.shape[1] != kernel.shape[1]):
            # pad the window with zeros if it is not the same size as the kernel
            window = np.pad(
                window,
                (
                    (0, kernel.shape[0] - window.shape[0]),
                    (0, kernel.shape[1] - window.shape[1]),
                ),
                "constant",
            )
        denominator = np.sum(kernel * window)
        denominator = denominator if denominator != 0 else 1e-9

        x_shift = np.sum((xi) * (kernel) * window) / denominator
        y_shift = np.sum((yi) * (kernel) * window) / denominator


        # Update the position
        x += x_shift
        y += y_shift

        path = np.vstack((path, [x, y]))

        # Check for convergence
        if np.sqrt(x_shift**2 + y_shift**2) < tol:
            break

    return path


# Example usage
data = generate_responses_1()  # Assuming this function is defined elsewhere

# has to be odd
kernel_size = 15
# Run mean-shift with different configurations and observe results
results = []
for kernel_type in ["epanechnikov"]:
    for bandwidth in [1]:
        kernel = np.ones((kernel_size, kernel_size))
        start_position = (30, 40)
        path = mean_shift(
            data.copy(), kernel, bandwidth=bandwidth, start_position=start_position
        )
        results.append((kernel_type, bandwidth, path))

# display the results as a path on the image
plt.imshow(data)
plt.plot(results[0][2][:, 0], results[0][2][:, 1], "r-")
plt.show()
