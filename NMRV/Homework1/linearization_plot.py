import numpy as np
import matplotlib.pyplot as plt

# Define the original function and its derivative
def f(x):
    return 2*x**2 + x**3

def f_prime(x):
    return 4*x + 3*x**2

# Define the linear approximation function
def linear_approximation(x, x0):
    return f(x0) + f_prime(x0) * (x - x0)

# Generate x values
x_values = np.linspace(-6, 6, 100)

# Calculate corresponding y values for the original function and linear approximation
y_original = f(x_values)
y_linear_approximation = linear_approximation(x_values, x0=4)  # Using x0=4 as the specified point

# Plot the original function and its linear approximation
plt.plot(x_values, y_original, label='Original Function: $2x^2 + x^3$')
plt.plot(x_values, y_linear_approximation, label='Linear Approximation', linestyle='--', color='red')

# Highlight the tangent point
plt.scatter([4], [f(4)], color='red')  # Point (4, f(4))

# Add labels and legend
plt.title('Original Function and Linear Approximation')
plt.xlabel('x')
plt.ylabel('y')
plt.legend()

# Display the plot
plt.grid(True)
plt.show()
