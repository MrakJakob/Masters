import time
import cv2
import numpy as np
import matplotlib.pyplot as plt
from ex1_utils import rotate_image, show_flow
from of_methods import lucas_kanade, horn_schunck

# Random images
im1=np.random.rand(200, 200).astype(np.float32)
im2=im1.copy()
im2=rotate_image(im2, 1)

# Read images
im1 = cv2.imread('./collision/00000152.jpg', cv2.IMREAD_GRAYSCALE).astype(np.float32)
im2 = cv2.imread('./collision/00000153.jpg', cv2.IMREAD_GRAYSCALE).astype(np.float32)

im1 = cv2.imread('./disparity/office2_left.png', cv2.IMREAD_GRAYSCALE).astype(np.float32)
im2 = cv2.imread('./disparity/office2_right.png', cv2.IMREAD_GRAYSCALE).astype(np.float32)

im1 = cv2.imread("./lab2/010.jpg", cv2.IMREAD_GRAYSCALE).astype(np.float32)
im2 = cv2.imread("./lab2/011.jpg", cv2.IMREAD_GRAYSCALE).astype(np.float32)

# # normalize images
im1 = cv2.normalize(im1, None, 0, 1, cv2.NORM_MINMAX)
im2 = cv2.normalize(im2, None, 0, 1, cv2.NORM_MINMAX)

# Lucas Kanade
start = time.time()
U_lk, V_lk=lucas_kanade(im1, im2, 14)
end = time.time()
print('Lucas Kanade: ', end - start)




# display all results in one plot
# fig, ((ax1, ax2), (ax3, ax4), (ax5, ax6), (ax7, ax8)) = plt.subplots(4, 2)
# ax1.imshow(im1)
# ax2.imshow(im2)
# show_flow(U_lk, V_lk, ax3, type='angle')
# show_flow(U_lk, V_lk, ax4, type='field', set_aspect=True)
# show_flow(U_lk_9, V_lk_9, ax5, type='angle')
# show_flow(U_lk_9, V_lk_9, ax6, type='field', set_aspect=True)
# show_flow(U_lk_14, V_lk_14, ax7, type='angle')
# show_flow(U_lk_14, V_lk_14, ax8, type='field', set_aspect=True)
# fig.suptitle('Lucas Kanade Optical Flow')

# plt.show()

# # Horn Schunck
start = time.time()
# U_hs, V_hs=horn_schunck(im1, im2, 1000, 0.5)

U_hs, V_hs=horn_schunck(im1, im2, 1000, 0.5, lucas=False, converge=False)
end = time.time()
print('Horn Schunck: ', end - start) 

# display the results
fig, ((ax1, ax2), (ax3, ax4)) = plt.subplots(2, 2)
ax1.imshow(im1)
ax2.imshow(im2)
show_flow(U_hs, V_hs, ax3, type='angle')
show_flow(U_hs, V_hs, ax4, type='field', set_aspect=True)
fig.suptitle('Horn Schunck Optical Flow')

# initialize with lucas kanade
start = time.time()
U_hs, V_hs=horn_schunck(im1, im2, 1000, 0.5, lucas=True, converge=False)
end = time.time()
print('Horn Schunck with Lucas input: ', end - start) 




fig1, ((ax1_11, ax1_12), (ax1_21, ax1_22)) =plt.subplots(2, 2)
ax1_11.imshow(im1)
ax1_12.imshow(im2)

show_flow(U_lk, V_lk, ax1_21, type='angle')
show_flow(U_lk, V_lk, ax1_22, type='field' , set_aspect=True)

fig1.suptitle('Lucas Kanade Optical Flow')
fig2, ((ax2_11, ax2_12), (ax2_21, ax2_22)) =plt.subplots(2, 2)

ax2_11.imshow(im1)
ax2_12.imshow(im2)

show_flow(U_hs, V_hs, ax2_21, type='angle')
show_flow(U_hs, V_hs, ax2_22, type='field' , set_aspect=True)

fig2.suptitle('Horn Schunck with Lucas input')
plt.show()