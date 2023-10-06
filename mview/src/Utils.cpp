#include <string>
#include <vector>
#include <iostream>

#include "GLFW/glfw3.h"
#include "mview.h"

#include "NativeFunctions.h"

MV_EXPORT const char* OpenFileDialog(const char* filter, void* owner)
{
#ifdef MV_WINDOWS
	return OpenFileDialogNativeWindows(filter, owner);
#elif defined MV_MACOSX
	return OpenFileDialogNativeMacOS(filter);
#endif
}

MV_EXPORT const char* SaveFileDialog(const char* filter, void* owner)
{

}

#define MAX(x, y) (x >= y ? x : y)
#define MIN(x, y) (x <= y ? x : y)

MV_EXPORT bool CenterWindow(GLFWwindow* window)
{
	if (!window)
		return false;

	int sx = 0, sy = 0;
	int px = 0, py = 0;
	int mx = 0, my = 0;
	int monitor_count = 0;
	int best_area = 0;
	int final_x = 0, final_y = 0;

	glfwGetWindowSize(window, &sx, &sy);
	glfwGetWindowPos(window, &px, &py);

	// Iterate through all monitors
	GLFWmonitor** m = glfwGetMonitors(&monitor_count);
	if (!m)
		return false;

	for (int j = 0; j < monitor_count; ++j)
	{

		glfwGetMonitorPos(m[j], &mx, &my);
		const GLFWvidmode* mode = glfwGetVideoMode(m[j]);
		if (!mode)
			continue;

		// Get intersection of two rectangles - screen and window
		int minX = MAX(mx, px);
		int minY = MAX(my, py);

		int maxX = MIN(mx + mode->width, px + sx);
		int maxY = MIN(my + mode->height, py + sy);

		// Calculate area of the intersection
		int area = MAX(maxX - minX, 0) * MAX(maxY - minY, 0);

		// If its bigger than actual (window covers more space on this monitor)
		if (area > best_area)
		{
			// Calculate proper position in this monitor
			final_x = mx + (mode->width - sx) / 2;
			final_y = my + (mode->height - sy) / 2;

			best_area = area;
		}

	}

	// We found something
	if (best_area)
		glfwSetWindowPos(window, final_x, final_y);

	// Something is wrong - current window has NOT any intersection with any monitors. Move it to the default one.
	else
	{
		GLFWmonitor* primary = glfwGetPrimaryMonitor();
		if (primary)
		{
			const GLFWvidmode* desktop = glfwGetVideoMode(primary);

			if (desktop)
				glfwSetWindowPos(window, (desktop->width - sx) / 2, (desktop->height - sy) / 2);
			else
				return false;
		}
		else
			return false;
	}

	return true;
}