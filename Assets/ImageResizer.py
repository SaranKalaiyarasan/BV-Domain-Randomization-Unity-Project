from PIL import Image
import os
import glob
from tqdm import tqdm

def determine_resampling(current_size, target_size):
    """
    Determines the appropriate resampling filter based on whether the image
    is being upscaled or downscaled.
    """
    if current_size[0] < target_size[0] or current_size[1] < target_size[1]:
        # Upscaling
        return Image.LANCZOS
    else:
        # Downscaling
        return Image.LANCZOS

def resize_image(image_path, output_folder, target_size):
    """
    Resizes an image to a target size and saves it to the output folder.
    """
    img = Image.open(image_path)
    resampling_filter = determine_resampling(img.size, target_size)
    resized_img = img.resize(target_size, resampling_filter)

    if not os.path.exists(output_folder):
        os.makedirs(output_folder)

    img_basename = os.path.basename(image_path)
    output_path = os.path.join(output_folder, img_basename)
    resized_img.save(output_path)

def resize_images_in_folder(input_folder, output_folder, target_size):
    """
    Resizes all .png images in the input folder to the target size and saves
    them to the output folder.
    """
    image_paths = glob.glob(os.path.join(input_folder, '*.png'))
    for image_path in tqdm(image_paths, desc="Resizing Images"):
        resize_image(image_path, output_folder, target_size)

if __name__ == "__main__":
    input_folder = "E:\\Data\\Buckeye Vertical\\Image Classifier\\Mar26\\valid\\imagesnoise"  # Update this path
    output_folder = "E:\\Data\\Buckeye Vertical\\Image Classifier\\Mar26\\valid\\resizedimages"  # Update this path
    target_size = (1280, 720)  # Set your target size here

    resize_images_in_folder(input_folder, output_folder, target_size)
