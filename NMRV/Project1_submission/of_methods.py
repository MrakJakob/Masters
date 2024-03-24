from cv2 import filter2D
import cv2
import numpy as np
from scipy.signal import convolve2d
from ex1_utils import gaussderiv, gausssmooth


def lucas_kanade(im1, im2, n):
    sigma = 1
    # Calculate spatial derivatives Ix and Iy
    Ix, Iy = gaussderiv(im1, sigma)
    Ix2, Iy2 = gaussderiv(im2, sigma)

    # Average spatial derivative in frame t and t+1
    Ix = 1/2 * (Ix + Ix2)
    Iy = 1/2 * (Iy + Iy2)
    
    # Calculate temporal derivative It
    It = im2 - im1
    # Improvement 1: Smooth the temporal derivative using a Gaussian filter
    It = gausssmooth(It, sigma)

    
    # HARISS CORNER DETECTION
    threshold = 5e-8  # 5e-7
    im1 = np.float32(im1)
    # Compute Harris response for image 1
    # harris_response = cv2.cornerHarris(im1, n, 3, 0.04)

    # Threshold the Harris response
    # mask = harris_response > threshold
   
    # Modify calculations to only use pixels where mask is True
    # Uncomment the following lines to use the mask for Harris corner detection
    # for i in range(im1.shape[0]):
    #     for j in range(im1.shape[1]):
    #         if not mask[i, j]:
    #             Ix[i, j] = 0
    #             Iy[i, j] = 0
    #             It[i, j] = 0


    # Define the convolution kernel for averaging in the neighborhood
    kernel = np.ones((n, n))

    # Calculate required sums by convolving with the kernel
    IxIx = filter2D(Ix * Ix, -1, kernel)
    IyIy = filter2D(Iy * Iy, -1, kernel)
    IxIy = filter2D(Ix * Iy, -1, kernel)
    IxIt = filter2D(Ix * It, -1, kernel)
    IyIt = filter2D(Iy * It, -1, kernel)


    # Calculate determinant of the covariance matrix
    D = IxIx * IyIy - IxIy**2
    # if D is zero, set it to a low number to avoid division by zero
    D[D == 0] = 1e-10

    # Calculate displacement vectors u and v
    u = (-IyIy*IxIt + IxIy*IyIt) / D
    v = (IxIy*IxIt - IxIx*IyIt) / D

    return u, v


def horn_schunck(im1, im2, n_iters=100, lmbd=1.0, lucas=False, converge=False):
    sigma = 1
    # Calculate spatial derivatives Ix and Iy
    Ix, Iy = gaussderiv(im1, sigma)
    Ix2, Iy2 = gaussderiv(im2, sigma)

    # Temporal derivative It
    It = im2 - im1

    # Improvement: Smooth the temporal derivative using a Gaussian filter
    It = gausssmooth(It, sigma)

    # Average spatial derivative in frame t and t+1
    Ix = 1/2 * (Ix + Ix2)
    Iy = 1/2 * (Iy + Iy2)

    # Initialize displacement vectors u and v
    u = np.zeros_like(im1)
    v = np.zeros_like(im1)

    # Define Laplacian kernel
    Ld = np.array([
        [0, 0.25, 0],
        [0.25, 0, 0.25],
        [0, 0.25, 0]
    ])

    # Improvement: Use Lucas-Kanade to initialize u and v
    if lucas:
        u, v = lucas_kanade(im1, im2, 14)

    prev_u = u
    prev_v = v

    tolerance = 1e-3
    max_iter = n_iters

    for i in range(n_iters):
        ua = filter2D(u, -1, Ld)
        va = filter2D(v, -1, Ld)

        P = Ix * ua + Iy * va + It
        D = lmbd + Ix**2 + Iy**2


        u = ua - Ix*(P / D)
        v = va - Iy*(P / D)

        # Uncomment to use the early stopping criterion
        # Stop if the average change is below a tolerance or reach max iterations

        if converge:
            du = np.mean(np.abs(u - prev_u))
            dv = np.mean(np.abs(v - prev_v))
            dtotal = du + dv

            # Stop if the average change is below a tolerance or reach max iterations
            if dtotal < tolerance or i == max_iter - 1:
                print(f'Converged after {i} iterations')
                break

            # Update for next iteration
            prev_u = u
            prev_v = v

    return u, v